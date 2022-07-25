// using Nike.Benchmark.ConsumerPerformance;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // services.AddHostedService<KafkaConsumerBackgroundService>();
    })
    .Build();

await host.RunAsync();
