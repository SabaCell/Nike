using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nike.Framework.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
       IEnumerable<IDomainEvent> GetChangedEvents();
        void Rollback();
    }
}