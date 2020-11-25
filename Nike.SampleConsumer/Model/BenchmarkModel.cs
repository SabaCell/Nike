using System;
using System.Text.Json;
using Nike.EventBus.Events;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Kafka;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Mediator.Handlers;

namespace Nike.SampleConsumer.Model
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
            // Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} ");

            return Task.CompletedTask;
        }
    }

    public class ConsumerHostedService1 : ConsumerHostedService
    {
        public ConsumerHostedService1(ILogger<ConsumerHostedService1> logger, IKafkaConsumerConnection connection,
            IServiceProvider services) : base(logger, connection, services)
        {
        }
    }

}