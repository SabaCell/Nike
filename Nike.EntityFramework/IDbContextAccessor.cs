using Microsoft.EntityFrameworkCore;

namespace Nike.EntityFramework
{
    public interface IDbContextAccessor
    {
         DbContext Context { get; }
    }
}