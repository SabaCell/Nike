using Nike.Framework.Domain;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nike.EntityFramework
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;


        public EfUnitOfWork(IDbContextAccessor dbContextAccessor)
        {
            _dbContext = dbContextAccessor.Context;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> CommitAsync()
        {
            var result = await _dbContext.SaveChangesAsync();

            return result;
        }

        public void Rollback()
        {
        }
    }
}