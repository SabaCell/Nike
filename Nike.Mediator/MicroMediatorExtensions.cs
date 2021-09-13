// using System.Linq;
// using System.Threading.Tasks;
// using Enexure.MicroBus;
// using Nike.Framework.Domain;
//
// namespace Nike.Mediator
// {
//     public static class MicroMediatorExtensions
//     {
//         public static async Task PublishEventsAsync(this IMicroMediator mediator, IAggregateRoot root)
//         {
//             foreach (var e in root.Events)
//             {
//                 await mediator.SendAsync(e);
//             }
//         }
//
//         public static async Task ApplyEvents(this IAggregateRoot root, IMicroMediator mediator)
//         {
//             var events = root.Events.ToList();
//
//             root.ClearEvents();
//
//             foreach (var e in events.AsParallel())
//             {
//                 await mediator.PublishEventAsync(e);
//             }
//         }
//
//         public static async Task PublishEventAsync(this IMicroMediator mediator, IDomainEvent @event)
//         {
//             await mediator.SendAsync(@event);
//         }
//     }
// }