using System.Reflection;
using Nike.EntityFramework;

namespace Nike.CustomerManagement.Infrastructure
{
    public class DatabaseContext : DbContextBase
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override Assembly ConfigurationsAssembly => typeof(Program).Assembly;
    }
}