using System.Threading.Tasks;

namespace Nike.OrderManagement.Projections.Framework;

public interface IEventBus
{
    Task PublishAsync<T>(T @event);
}