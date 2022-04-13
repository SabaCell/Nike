using System;
using System.Linq.Expressions;

namespace Nike.Framework.Domain.Specifications;

public interface ISpecification<TEntity>
{
    Expression<Func<TEntity, bool>> Criteria { get; }
}