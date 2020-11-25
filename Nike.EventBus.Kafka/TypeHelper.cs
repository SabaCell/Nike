using System;
using System.Linq;
using Confluent.Kafka;
using System.Text.Json;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Nike.EventBus.Kafka.Model;

namespace Nike.EventBus.Kafka
{
    public static class KafkaConsumerHelper
    {
        public static bool TryConsumeMessage(this IConsumer<Ignore, string> consumer, ILogger logger,
            ConsumeMessageResult result, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (!TryConsume(consumer, logger, out var consumeResult, millisecondsTimeout,
                cancellationToken))
                return false;

            result.SetMessageAsync(consumeResult);

            return true;
        }

        public static bool TryConsume(this IConsumer<Ignore, string> consumer, ILogger logger,
            out ConsumeResult<Ignore, string> consumeResult, int millisecondsTimeout,
            CancellationToken cancellationToken)
        {
            consumeResult = null;

            try
            {
                consumeResult = consumer.Consume(millisecondsTimeout);

                if (consumeResult == null)
                {
                    consumeResult = consumer.Consume(cancellationToken);
                 
                    if (consumeResult == null)
                    {
                        logger.LogTrace($"{consumer.Name} is empty");
                        return false;
                    }
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


    // public class TimeTracker
    // {
    //     private static readonly Dictionary<string, Stopwatch> Stopwatches = new Dictionary<string, Stopwatch>();
    //
    //     public static void Start(string key)
    //     {
    //         Stopwatches.Add(key, Stopwatch.StartNew());
    //     }
    //
    //     public static void Stop(string key)
    //     {
    //         Stopwatches[key].Stop();
    //     }
    //
    //     public static void Clear()
    //     {
    //         Stopwatches.Clear();
    //     }
    //
    //     public static Dictionary<string, double> Conclusion()
    //     {
    //         var temp = Stopwatches.ToList();
    //
    //         return temp.ToDictionary(stopwatch => stopwatch.Key,
    //             stopwatch => stopwatch.Value.Elapsed.TotalMilliseconds);
    //     }
    // }
    //
    public class TimeTrackerCollection
    {
        private static readonly Dictionary<string, List<double>> _total = new Dictionary<string, List<double>>();

        public static void Append(Dictionary<string, double> conclusion)
        {
            foreach (var (key, value) in conclusion)
            {
                if (_total.ContainsKey(key))
                    _total[key].Add(value);
                else
                    _total.Add(key, new List<double> {value});
            }
        }

        private static List<Tuple<string, Tuple<int, double, double, double>>> ShowResult()
        {
            return _total.Select(t => new Tuple<string, Tuple<int, double, double, double>>(t.Key,
                    new Tuple<int, double, double, double>(t.Value.Count, t.Value.Min(), t.Value.Average(),
                        t.Value.Max())))
                .ToList();
        }

        public static void Print()
        {
            foreach (var r in ShowResult())
            {
                var str =
                    $"'key':'{r.Item1}', 'cnt':{r.Item2.Item1}, 'min':{r.Item2.Item2}, 'avg':{r.Item2.Item3}, 'max':{r.Item2.Item4}";

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{ " + str + " }");

                Console.ForegroundColor = ConsoleColor.White;
            }

            _total.Clear();
        }
    }
}