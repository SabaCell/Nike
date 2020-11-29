using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Events;
using Nike.EventBus.Kafka;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Mediator.Handlers;

namespace Nike.SampleConsumer2.Model
{
    public class Msg1 : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class Msg1IntegrationEventHandler : IntegrationEventHandler<Msg1>
    {
        public override Task HandleAsync(Msg1 @event)
        {
//            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
            return Task.CompletedTask;
        }
    }

    public class Msg2 : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class Msg2IntegrationEventHandler : IntegrationEventHandler<Msg2>
    {
        public override Task HandleAsync(Msg2 @event)
        {
//            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
            return Task.CompletedTask;
        }
    }

    public class Msg3 : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class Msg3IntegrationEventHandler : IntegrationEventHandler<Msg3>
    {
        public override Task HandleAsync(Msg3 @event)
        {
//            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
            return Task.CompletedTask;
        }
    }

    public class Msg4 : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class Msg4IntegrationEventHandler : IntegrationEventHandler<Msg4>
    {
        public override Task HandleAsync(Msg4 @event)
        {
//            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
            return Task.CompletedTask;
        }
    }

    public class Msg5 : IntegrationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class Msg5IntegrationEventHandler : IntegrationEventHandler<Msg5>
    {
        public override Task HandleAsync(Msg5 @event)
        {
//            Console.WriteLine($"Consumer1: {@event.Count} - {@event.Id} - {@event.GetType().Name} ");
            return Task.CompletedTask;
        }
    }

    public class ConsumerHostedService21 : ConsumerHostedService
    {
        public ConsumerHostedService21(ILogger<ConsumerHostedService21> logger, IKafkaConsumerConnection connection,
            IServiceProvider services) : base(logger, connection, services)
        {
        }
    }

    public class ConsumerHostedService22 : ConsumerHostedService
    {
        public ConsumerHostedService22(ILogger<ConsumerHostedService22> logger, IKafkaConsumerConnection connection,
            IServiceProvider services) : base(logger, connection, services)
        {
        }
    }

    public class ConsumerHostedService23 : ConsumerHostedService
    {
        public ConsumerHostedService23(ILogger<ConsumerHostedService23> logger, IKafkaConsumerConnection connection,
            IServiceProvider services) : base(logger, connection, services)
        {
        }
    }
}