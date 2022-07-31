using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nike.Framework.Domain.Specifications;

namespace Nike.Framework.Domain.Persistence;

/// <summary>
///     the repository support specification pattern. please refer to
///     <a href="https://deviq.com/specification-pattern/">DevIQ Specification Pattern Article</a>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    void Add(TEntity entity);
    Task<TEntity> GetByIdAsync<TPrimaryKey>(TPrimaryKey primaryKey);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<TEntity> GetSingleAsync(ISpecification<TEntity> specification);
    Task<List<TEntity>> GetAllAsync();
    IQueryable<TEntity> GetQueryable();

    IQueryable<TEntity> Pagination(int pageIndex, int pageSize);
    IAsyncEnumerable<TEntity> GetAsyncEnumerable(ISpecification<TEntity> specification);
    Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification);
    Task<bool> IsExistAsync(ISpecification<TEntity> specification);
    Task<int> GetCountAsync(ISpecification<TEntity> specification);
}