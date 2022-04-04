using System;
using System.Threading.Tasks;

namespace Nike.Framework.Domain.EventSourcing;

public interface IRepository<TEntity, TKey> where TEntity : AggregateRoot<TKey> where TKey : IEquatable<TKey>
{
    Task AddAsync(TEntity aggregateRoot);
    Task<TEntity> GetByIdAsync(TKey id);
}