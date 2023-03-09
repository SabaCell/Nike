using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nike.Framework.Domain.Specifications;

public interface IRelationalSpecification<TEntity> : ISpecification<TEntity>
{
    List<Expression<Func<TEntity, object>>> Includes { get; }
    Expression<Func<TEntity, object>> OrderBy { get; }
    Expression<Func<TEntity, object>> OrderByDescending { get; }
    public Expression<Func<TEntity, object>> GroupBy { get;  }
    int Skip { get; }
    int Take { get; }
    bool TrackingEnabled { get; }
    void AddInclude(Expression<Func<TEntity, object>> include);
    public void AddInclude(string include);
    void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy);
    void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescending);
    void ApplySkipTake(int skip, int take);
    void ApplyCriteria(Expression<Func<TEntity, bool>> criteria);
    void ApplyGroupBy(Expression<Func<TEntity, object>> groupByExpression);
}