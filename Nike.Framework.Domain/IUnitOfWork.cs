using System;
using System.Threading.Tasks;

namespace Nike.Framework.Domain;

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
    void Rollback();
}