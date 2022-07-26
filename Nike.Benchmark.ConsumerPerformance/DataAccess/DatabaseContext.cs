using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nike.Benchmark.ConsumerPerformance.Models;
using Nike.EntityFramework;

public class DatabaseContext : DbContextBase
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override Assembly ConfigurationsAssembly => typeof(Program).Assembly;
}



