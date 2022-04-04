using System.Threading.Tasks;

namespace Nike.OrderManagement.Projections.Framework;

public interface IEventHandler<T>
{
    Task HandleAsync(T @event);
}