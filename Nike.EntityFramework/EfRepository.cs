using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nike.Framework.Domain.Persistence;
using Nike.Framework.Domain.Specifications;

namespace Nike.EntityFramework;

public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public EfRepository(IDbContextAccessor contextAccessor)
    {
        Context = contextAccessor.Context;
        DbSet = Context.Set<TEntity>();
    }

    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public async Task<TEntity> GetByIdAsync<TPrimaryKey>(TPrimaryKey primaryKey)
    {
        return await DbSet.FindAsync(primaryKey);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public Task<TEntity> GetSingleAsync(ISpecification<TEntity> specification)
    {
        var query = ApplySpecification(specification);

        return query.SingleOrDefaultAsync();
    }

    public IAsyncEnumerable<TEntity> GetAsyncEnumerable(ISpecification<TEntity> specification)
    {
        var query = ApplySpecification(specification);

        return query.AsAsyncEnumerable();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification)
    {
        var query = ApplySpecification(specification);

        return await query.ToListAsync();
    }

    public Task<int> GetCountAsync(ISpecification<TEntity> specification)
    {
        var query = ApplySpecification(specification);

        return query.CountAsync();
    }

    public Task<bool> IsExistAsync(ISpecification<TEntity> specification)
    {
        var query = ApplySpecification(specification);

        return query.AnyAsync();
    }


    public Task<List<TEntity>> GetAllAsync()
    {
        return DbSet.IgnoreAutoIncludes().ToListAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        var query = DbSet.AsQueryable();

        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);

        if (specification is IRelationalSpecification<TEntity> relationalSpecification)
            query = query.Specify(relationalSpecification);

        return query;
    }
}