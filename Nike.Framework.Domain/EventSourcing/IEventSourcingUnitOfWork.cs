using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nike.Framework.Domain.EventSourcing
{
    public interface IEventSourcingUnitOfWork : IUnitOfWork
    {
        Task AddEvents(string stream, IEnumerable<DomainEvent> events);

        IEnumerable<DomainEvent> GetUncommittedEvents(string stream);
    }
}