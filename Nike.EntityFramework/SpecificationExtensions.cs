using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nike.Framework.Domain.Specifications;

namespace Nike.EntityFramework;

public static class SpecificationExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> queryable, IRelationalSpecification<T> specification)
        where T : class
    {
        return queryable
            .SetTracking(specification)
            .Include(specification)
            .Where(specification)
            .OrderBy(specification)
            .OrderByDescending(specification)
            .SkipTake(specification);
    }

    private static IQueryable<T> Include<T>(this IQueryable<T> queryable, IRelationalSpecification<T> specification)
        where T : class
    {
        return specification.Includes.Aggregate(queryable, (current, include) => current.Include(include));
    }

    private static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, IRelationalSpecification<T> specification)
        where T : class
    {
        return specification.OrderBy == default || specification.OrderByDescending != default
            ? queryable
            : queryable.OrderBy(specification.OrderBy);
    }

    private static IQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable,
        IRelationalSpecification<T> specification) where T : class
    {
        return specification.OrderByDescending == default || specification.OrderBy != default
            ? queryable
            : queryable.OrderByDescending(specification.OrderByDescending);
    }

    private static IQueryable<T> SkipTake<T>(this IQueryable<T> queryable,
        IRelationalSpecification<T> specification) where T : class
    {
        return specification.Take == 0 ? queryable : queryable.Skip(specification.Skip).Take(specification.Take);
    }

    private static IQueryable<T> Where<T>(this IQueryable<T> queryable, IRelationalSpecification<T> specification)
        where T : class
    {
        return specification.Criteria == default ? queryable : queryable.Where(specification.Criteria);
    }

    private static IQueryable<T> SetTracking<T>(this IQueryable<T> queryable,
        IRelationalSpecification<T> specification) where T : class
    {
        return specification.TrackingEnabled ? queryable : queryable.AsNoTracking();
    }
}