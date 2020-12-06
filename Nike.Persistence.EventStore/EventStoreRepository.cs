using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Nike.Framework.Domain.EventSourcing;

namespace Nike.Persistence.EventStore
{
    public class EventStoreRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : AggregateRoot<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IEventStoreConnection _connection;
        private readonly IEventTypeResolver _eventTypeResolver;

        public EventStoreRepository(IEventTypeResolver eventTypeResolver, IEventStoreConnection connection)
        {
            _connection = connection;
            _eventTypeResolver = eventTypeResolver;
        }

        public async Task AddAsync(TEntity aggregateRoot)
        {
            var events = EventDataFactory.CreateFromDomainEvents(aggregateRoot.GetChanges());

            var streamId = GenerateStreamId(aggregateRoot.Id);
            await _connection.AppendToStreamAsync(streamId, ExpectedVersion.Any, events);
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            var streamId = GenerateStreamId(id);
            var streamEvents = await EventStreamReader.Read(_connection, streamId, StreamPosition.Start, 200);
            var domainEvents = DomainEventFactory.Create(streamEvents, _eventTypeResolver);

            return AggregateFactory.Create<TEntity>(domainEvents);
        }

        #region PrivateMethods

        private static string GenerateStreamId(TKey id)
        {
            return $"{typeof(TEntity).Name}-{id}";
        }

        #endregion
    }
}