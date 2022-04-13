using System;
using System.Collections.Generic;
using System.Linq;

namespace Nike.EventBus.Kafka.Extenstion;

public class TimeTrackerCollection
{
    private static readonly Dictionary<string, List<double>> _total = new();

    public static void Append(Dictionary<string, double> conclusion)
    {
        foreach (var (key, value) in conclusion)
            if (_total.ContainsKey(key))
                _total[key].Add(value);
            else
                _total.Add(key, new List<double> {value});
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
        var json = "";
        foreach (var r in ShowResult())
        {
            var str =
                $"'key':'{r.Item1}', 'cnt':{r.Item2.Item1}, 'min':{r.Item2.Item2}, 'avg':{r.Item2.Item3}, 'max':{r.Item2.Item4}";

            Console.ForegroundColor = ConsoleColor.Green;
            str = "{ " + str + " }";
            Console.WriteLine(str);

            Console.ForegroundColor = ConsoleColor.White;

            json = json + "\r\n" + str;
        }

        _total.Clear();
        //File.AppendAllText(@"D:\consumerTimer.txt", json);
    }
}