using EventStore.ClientAPI;
using Newtonsoft.Json;
using Nike.OrderManagement.Domain.Contracts;
using Nike.OrderManagement.Projections.Framework;
using Nike.Persistence.EventStore;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Nike.OrderManagement.Projections
{
    class Program
    {
        private static readonly IEventTypeResolver TypeResolver = new EventTypeResolver();
        private static readonly IEventBus Bus = new InMemoryEventBus();

        private static void Main(string[] args)
        {
            TypeResolver.AddTypesFromAssembly(typeof(OrderPlaced).Assembly);

            var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@127.0.0.1:1113"));
            connection.ConnectAsync().Wait();

            connection.SubscribeToAllFrom(Position.Start, CatchUpSubscriptionSettings.Default,
                EventAppeared, LiveProcessingStarted, SubscriptionDropped);

            Console.WriteLine("Subscribed !");

            Console.ReadLine();
        }

        private static void SubscriptionDropped(EventStoreCatchUpSubscription arg1, SubscriptionDropReason arg2, Exception arg3)
        {
            Console.WriteLine("Subscription Dropped !");
            Console.WriteLine("--------------------------------");
        }
        private static void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            Console.WriteLine("Live Processing Started !");
            Console.WriteLine("--------------------------------");
        }
        private static Task EventAppeared(EventStoreCatchUpSubscription arg1, ResolvedEvent arg2)
        {
            if (IsSystemEvent(arg2)) return Task.CompletedTask;

            var typeOfEvent = TypeResolver.GetType(arg2.Event.EventType);
            if (typeOfEvent != null)
            {
                var body = Encoding.UTF8.GetString(arg2.Event.Data);
                var domainEvent = JsonConvert.DeserializeObject(body, typeOfEvent);

                Bus.PublishAsync((dynamic)domainEvent);
            }

            return Task.CompletedTask;
        }
        private static bool IsSystemEvent(ResolvedEvent arg2)
        {
            return arg2.Event.EventType.StartsWith("$");
        }
    }
}
