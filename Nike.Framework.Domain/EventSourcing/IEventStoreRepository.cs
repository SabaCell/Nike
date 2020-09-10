using System.Threading.Tasks;

namespace Nike.Framework.Domain.EventSourcing
{
    public interface IEventStoreRepository
    {
        Task<T> LoadAsync<T>(string aggregateRootId) where T : EventSourcedAggregateRoot, new();

        Task SaveAsync<T>(T aggregate) where T : EventSourcedAggregateRoot, new();
    }
}