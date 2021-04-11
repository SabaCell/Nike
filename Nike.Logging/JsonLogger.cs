using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Nike.Logging
{
    public class JsonLogger : ILogger
    {
        private readonly TextWriter _writer;
        private readonly string _categoryName;

        private IExternalScopeProvider ScopeProvider { get; set; }

        public JsonLogger(TextWriter writer, string categoryName, IExternalScopeProvider scopeProvider)
        {
            _writer = writer;
            _categoryName = categoryName;
            ScopeProvider = scopeProvider;
        }
        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));

            var message = new JsonLogEntry
            {
                Timestamp = DateTime.UtcNow,
                LogLevel = logLevel,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Category = _categoryName,
                Exception = exception?.ToString(),
                Message = formatter(state, exception),
            };


            AppendScope(message.Scope, state);
            AppendScope(message.Scope);
            var options = new JsonSerializerOptions {Converters = {new JsonStringEnumConverter()}};
            _writer.WriteLine(JsonSerializer.Serialize(message, options));
        }

        private void AppendScope(IDictionary<string, object> dictionary)
        {
            ScopeProvider.ForEachScope((scope, state) => AppendScope(state, scope), dictionary);
        }

        private static void AppendScope(IDictionary<string, object> dictionary, object scope)
        {
            switch (scope)
            {
                case null:
                    return;
                case IReadOnlyList<KeyValuePair<string, object>> formattedLogValues:
                {
                    if (formattedLogValues.Count > 0)
                    {
                        foreach (var value in formattedLogValues)
                        {
                            if (value.Value is MethodInfo)
                                continue;

                            dictionary[value.Key] = value.Value;
                        }
                    }

                    break;
                }
                default:
                {
                    var appendToDictionaryMethod = ExpressionCache.GetOrCreateAppendToDictionaryMethod(scope.GetType());
                    appendToDictionaryMethod(dictionary, scope);
                    break;
                }
            }
        }


        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();

            private NullScope()
            {
            }

            public void Dispose()
            {
            }
        }


        private static class ExpressionCache
        {
            public delegate void AppendToDictionary(IDictionary<string, object> dictionary, object o);

            private static readonly ConcurrentDictionary<Type, AppendToDictionary> STypeCache =
                new ConcurrentDictionary<Type, AppendToDictionary>();

            private static readonly PropertyInfo DictionaryIndexerProperty = GetDictionaryIndexer();

            public static AppendToDictionary GetOrCreateAppendToDictionaryMethod(Type type)
            {
                return STypeCache.GetOrAdd(type, CreateAppendToDictionaryMethod);
            }

            private static AppendToDictionary CreateAppendToDictionaryMethod(Type type)
            {
                var dictionaryParameter = Expression.Parameter(typeof(IDictionary<string, object>), "dictionary");
                var objectParameter = Expression.Parameter(typeof(object), "o");

                var castedParameter = Expression.Convert(objectParameter, type); // cast o to the actual type

                // Create setter for each properties
                // dictionary["PropertyName"] = o.PropertyName;
                var properties =
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                var setters =
                    from prop in properties
                    where prop.CanRead
                    let indexerExpression = Expression.Property(dictionaryParameter, DictionaryIndexerProperty,
                        Expression.Constant(prop.Name))
                    let getExpression = Expression.Property(castedParameter, prop.GetMethod)
                    select Expression.Assign(indexerExpression, getExpression);

                var body = new List<Expression>() {castedParameter};
                body.AddRange(setters);

                var lambdaExpression =
                    Expression.Lambda<AppendToDictionary>(Expression.Block(body), dictionaryParameter, objectParameter);
                return lambdaExpression.Compile();
            }

            // Get the PropertyInfo for IDictionary<string, object>.this[string key]
            private static PropertyInfo GetDictionaryIndexer()
            {
                var indexers =
                    from prop in typeof(IDictionary<string, object>).GetProperties(BindingFlags.Instance |
                        BindingFlags.Public)
                    let indexParameters = prop.GetIndexParameters()
                    where indexParameters.Length == 1 &&
                          typeof(string).IsAssignableFrom(indexParameters[0].ParameterType)
                    select prop;

                return indexers.Single();
            }
        }
    }
}