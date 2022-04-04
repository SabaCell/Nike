using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nike.Framework.Domain.Persistence;

namespace Nike.Framework.Domain.Specifications;

public class RelationalSpecification<T> : IRelationalSpecification<T>
{
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public bool TrackingEnabled { get; private set; } = true;
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public Expression<Func<T, bool>> Criteria { get; private set; }

    public void AddInclude(Expression<Func<T, object>> include)
    {
        Includes.Add(include);
    }

    public void AddInclude(string include)
    {
        Includes.Add(GetLambda<T>(include));
    }

    public void ApplyOrderBy(Expression<Func<T, object>> orderBy)
    {
        OrderBy = orderBy;
    }

    public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescending)
    {
        OrderByDescending = orderByDescending;
    }

    public void ApplySkipTake(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    public void ApplyCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public void DisableTracking()
    {
        TrackingEnabled = false;
    }

    private static Expression<Func<T, object>> GetLambda<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.PropertyOrField(parameter, propertyName);
        return Expression.Lambda<Func<T, object>>(property, parameter);
    }
}

public static class RelationalSpecificationExtenstion
{
    public static RelationalSpecification<TEntity> Create<TEntity>(this IRepository<TEntity> repository)
        where TEntity : class
    {
        return new RelationalSpecification<TEntity>();
    }

    public static RelationalSpecification<TEntity> Include<TEntity>(
        this RelationalSpecification<TEntity> specification,
        Expression<Func<TEntity, object>> include)
    {
        specification.AddInclude(include);

        return specification;
    }

    public static RelationalSpecification<TEntity> OrderBy<TEntity>(
        this RelationalSpecification<TEntity> specification,
        Expression<Func<TEntity, object>> orderBy)
    {
        specification.ApplyOrderBy(orderBy);

        return specification;
    }

    public static RelationalSpecification<TEntity> OrderByDescending<TEntity>(
        this RelationalSpecification<TEntity> specification, Expression<Func<TEntity, object>> orderByDescending)
    {
        specification.ApplyOrderByDescending(orderByDescending);

        return specification;
    }

    public static RelationalSpecification<TEntity> SkipTake<TEntity>(
        this RelationalSpecification<TEntity> specification,
        int skip, int take)
    {
        specification.ApplySkipTake(skip, take);

        return specification;
    }

    public static RelationalSpecification<TEntity> Criteria<TEntity>(
        this RelationalSpecification<TEntity> specification,
        Expression<Func<TEntity, bool>> criteria)
    {
        specification.ApplyCriteria(criteria);

        return specification;
    }
}