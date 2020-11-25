using System;
using System.Text.Json;
using Nike.EventBus.Events;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Kafka;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Mediator.Handlers;

namespace Nike.SampleConsumer2.Model
{
    public class bncMsgIntegrationEvent : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class bncMsgIntegrationEventHandler : IntegrationEventHandler<bncMsgIntegrationEvent>
    {
        public override Task HandleAsync(bncMsgIntegrationEvent @event)
        {
            if (new Random(1000).Next(1000000) / 5 == 0)
                Console.WriteLine($"Consumer2: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");

            return Task.CompletedTask;
        }
    }


    public class bncMsg2IntegrationEvent : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class bncMsg2IntegrationEventHandler : IntegrationEventHandler<bncMsg2IntegrationEvent>
    {
        public override Task HandleAsync(bncMsg2IntegrationEvent @event)
        {
            if (new Random(1000).Next(1000000) / 5 == 0)
                Console.WriteLine($"Consumer2: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");

            return Task.CompletedTask;
        }
    }


    public class bncMsg3IntegrationEvent : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class bncMsg3IntegrationEventHandler : IntegrationEventHandler<bncMsg3IntegrationEvent>
    {
        public override Task HandleAsync(bncMsg3IntegrationEvent @event)
        {
            if (new Random(1000).Next(1000000) / 11 == 0)
                Console.WriteLine($"Consumer2: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
            return Task.CompletedTask;
        }
    }


    public class bncMsg4IntegrationEvent : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class bncMsg4IntegrationEventHandler : IntegrationEventHandler<bncMsg4IntegrationEvent>
    {
        public override Task HandleAsync(bncMsg4IntegrationEvent @event)
        {
            if (new Random(1000).Next(1000000) / 5 == 0)
                Console.WriteLine($"Consumer2: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
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