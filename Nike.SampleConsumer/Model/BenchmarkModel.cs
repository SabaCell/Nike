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
    
    public class MyMessage2Part : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class MyMessage2PartIntegrationEventHandler : IntegrationEventHandler<MyMessage2Part>
    {
        public override Task HandleAsync(MyMessage2Part @event)
        {
            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
            return Task.CompletedTask;
        }
    }

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
            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
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
            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");

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
            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
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
            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
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
    public class ConsumerHostedService2 : ConsumerHostedService
    {
        public ConsumerHostedService2(ILogger<ConsumerHostedService3> logger, IKafkaConsumerConnection connection,
            IServiceProvider services) : base(logger, connection, services)
        {
        }
    }
    public class ConsumerHostedService3 : ConsumerHostedService
    {
        public ConsumerHostedService3(ILogger<ConsumerHostedService3> logger, IKafkaConsumerConnection connection,
            IServiceProvider services) : base(logger, connection, services)
        {
        }
    }
}