using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Events;
using Nike.EventBus.Kafka;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Mediator.Handlers;

namespace Nike.SampleConsumer2.Model
{
    public class BenchmarkModelIntegrationEvent : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class BenchmarkModelIntegrationEventHandler : IntegrationEventHandler<BenchmarkModelIntegrationEvent>
    {
        public override Task HandleAsync(BenchmarkModelIntegrationEvent @event)
        {
            Console.WriteLine($"Consumer2: {@event.Count} - {@event.Id} ");

            return Task.CompletedTask;
        }
    }

    public class ConsumerHostedService2 : ConsumerHostedService
    {
        public ConsumerHostedService2(ILogger<ConsumerHostedService2> logger, IKafkaConsumerConnection connection,
            IServiceProvider services) : base(logger, connection, services)
        {
        }
    }
}