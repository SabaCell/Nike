﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nike.Framework.Domain;
using Nike.Framework.Domain.EntityEvents;

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


        // public IEnumerable<IDomainEvent> GetUncommittedEvents()
        // {
        //     var aggregateRoots = _dbContext.ChangeTracker
        //     .Entries<IAggregateRoot>()
        //     .Where(x => x.Entity.Events.Any())
        //     .ToList();
        //
        //     var domainEvents = aggregateRoots
        //     .SelectMany(x => x.Entity.Events)
        //     .ToList();
        //
        //     foreach (var entity in aggregateRoots) entity.Entity.ClearEvents();
        //
        //     return domainEvents;
        // }
        public IEnumerable<IDomainEvent> GetChangedEvents()
        {
            var events = new List<IDomainEvent>();
            var changes = _dbContext.ChangeTracker.Entries<IAggregateRoot>().Where(e => e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted)
                .ToList();

            if (changes.Count <= 0) return events;
            foreach (var change in changes)
            {
                switch (change.State)
                {
                    case EntityState.Added:
                        var @event = CreateDynamicEvent(typeof(AddedEntityDomainEvent<>), change.Entity);

                        events.Add(@event);
                        break;

                    case EntityState.Modified:
                        @event = CreateDynamicEvent(typeof(ModifiedEntityDomainEvent<>), change.Entity);
                        events.Add(@event);
                        break;
                    case EntityState.Deleted:
                        @event = CreateDynamicEvent(typeof(DeletedEntityDomainEvent<>), change.Entity);
                        events.Add(@event);

                        break;
                }
            }

            return events;
        }

        private dynamic CreateDynamicEvent(Type domainType, IAggregateRoot entity)
        {
            var type = domainType.MakeGenericType(entity.GetType());
            return Activator.CreateInstance(type, entity);
        }

        public void Rollback()
        {
        }
    }
}