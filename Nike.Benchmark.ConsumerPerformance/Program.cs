// using Nike.Benchmark.ConsumerPerformance;

namespace Nike.Benchmark.ConsumerPerformance;

class Program
{
    
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            // services.AddHostedService<KafkaConsumerBackgroundService>();
        })
        .Build();

    await host.RunAsync();
}