using System;
using System.Linq;
using Confluent.Kafka;
using System.Text.Json;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Nike.EventBus.Kafka
{
    public static class KafkaConsumerHelper
    {
        public static bool TryConsumeMessage<T>(this IConsumer<Ignore, string> consumer, ILogger logger,
            out object serializedMessage, int millisecondsTimeout = 1000,
            CancellationToken cancellationToken = default)
        {
            serializedMessage = null;

            if (!TryConsume(consumer, logger, out var consumeResult, millisecondsTimeout,
                cancellationToken))
            {
                return false;
            }

            serializedMessage = JsonSerializer.Deserialize(consumeResult.Message.Value, typeof(T));

            return true;
        }

        public static bool TryConsume(this IConsumer<Ignore, string> consumer, ILogger logger,
            out ConsumeResult<Ignore, string> consumeResult, int millisecondsTimeout = 1000,
            CancellationToken cancellationToken = default)
        {
            try
            {
                consumeResult = consumer.Consume(millisecondsTimeout);

                if (consumeResult == null)
                {
                    logger.LogTrace($"Kafka consumer Result is empty ");
                    return false;
                }

                if (consumeResult.Message == null)
                {
                    logger.LogTrace(
                        $"Kafka consumer is empty: {consumeResult.Topic}-{consumeResult.Offset}-{consumeResult.IsPartitionEOF}");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                consumeResult = null;
                logger.LogError(e.Message);
                cancellationToken.ThrowIfCancellationRequested();
                return false;
            }
        }
    }

    public class TypeHelper
    {
        private readonly WeakReference<IList<Type>> _types = new WeakReference<IList<Type>>(null);
        private readonly string exceptTypesRegex = "(System.*|Microsoft.*)";

        private IEnumerable<Type> Types
        {
            get
            {
                IList<Type> types;
                if (!_types.TryGetTarget(out types))
                {
                    types = new List<Type>();
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var count = assemblies.Length;
                    foreach (var assembly in assemblies)
                    {
                        var assemblyName = assembly.GetName().FullName;

                        if (!assembly.IsDynamic)
                        {
                            Type[] exportedTypes = null;
                            try
                            {
                                exportedTypes = assembly.GetExportedTypes();
                            }
                            catch (ReflectionTypeLoadException e)
                            {
                                exportedTypes = e.Types;
                            }

                            if (exportedTypes != null)
                                foreach (var exportedType in exportedTypes)
                                    if (!Regex.IsMatch(exportedType.FullName, exceptTypesRegex))
                                        types.Add(exportedType);
                        }
                    }

                    _types.SetTarget(types);
                }

                return types;
            }
        }

        public Type GetType(string typeFullName)
        {
            return Types.FirstOrDefault(type => type.FullName == typeFullName);
        }
    }
}