using System;
using System.Threading;
using System.Threading.Tasks;
using Nike.Framework.Domain.Events;

namespace Nike.Framework.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task PublishEventsAsync(CommitTime commitTime = (CommitTime.BeforeCommit | CommitTime.AfterCommit), CancellationToken cancellationToken = default);
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
        void Rollback(CancellationToken cancellationToken = default);
    }
}