﻿using System.Threading.Tasks;
using Nike.Framework.Domain;

namespace Nike.EntityFramework;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly DbContextBase _dbContext;


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