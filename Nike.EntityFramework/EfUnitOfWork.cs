﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nike.Framework.Domain;

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
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IEnumerable<IDomainEvent> GetUncommittedEvents()
        {
            var aggregateRoots = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.Events.Any())
            .ToList();

            var domainEvents = aggregateRoots
            .SelectMany(x => x.Entity.Events)
            .ToList();

            foreach (var entity in aggregateRoots) entity.Entity.ClearEvents();

            return domainEvents;
        }

        public void Rollback()
        {
        }
    }
}