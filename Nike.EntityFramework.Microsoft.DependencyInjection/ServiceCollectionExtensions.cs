using Microsoft.Extensions.DependencyInjection;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Persistence;

namespace Nike.EntityFramework.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkUnitOfWork(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return serviceCollection;
    }

    public static IServiceCollection AddEntityFrameworkDefaultRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return serviceCollection;
    }

    // public static IServiceCollection AddSqlServerDatabaseContext<TContext>(this IServiceCollection serviceCollection, string dbConnectionString) where TContext : DbContext
    // {
    //     serviceCollection.AddDbContext<TContext>(options =>
    //         options.UseSqlServer(dbConnectionString));
    //
    //     serviceCollection.AddScoped<IDbContextAccessor>(s =>
    //     {
    //         var dbContext = s.GetRequiredService<TContext>();
    //         return new DbContextAccessor(dbContext);
    //     });
    //     
    //     return serviceCollection;
    // }
}