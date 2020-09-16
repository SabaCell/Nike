using Microsoft.EntityFrameworkCore;
using Nike.EntityFramework;
using System.Reflection;

namespace Nike.CustomerManagement.Infrastructure
{
    public class DatabaseContext : DbContextBase<DatabaseContext>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override Assembly ConfigurationsAssembly => typeof(Program).Assembly;
    }
}