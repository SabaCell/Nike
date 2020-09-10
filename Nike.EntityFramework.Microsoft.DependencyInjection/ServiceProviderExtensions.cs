using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Nike.EntityFramework.Microsoft.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static void Migrate(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var dbContext = scope.ServiceProvider.GetService<IDbContextAccessor>().Context;

            dbContext.Database.Migrate();
        }

        public static async Task MigrateAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var dbContext = scope.ServiceProvider.GetService<IDbContextAccessor>().Context;

            await dbContext.Database.MigrateAsync();
        }
    }
}