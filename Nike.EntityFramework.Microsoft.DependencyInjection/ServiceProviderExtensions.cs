using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Nike.EntityFramework.Microsoft.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static void Migrate(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var contextAccessor = scope.ServiceProvider.GetService<IDbContextAccessor>();
            contextAccessor?.Context.Database.Migrate();
        }

        public static async Task MigrateAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var contextAccessor = scope.ServiceProvider.GetService<IDbContextAccessor>();
            var context = contextAccessor?.Context;

            await context?.Database.MigrateAsync(CancellationToken.None);
        }
    }
}