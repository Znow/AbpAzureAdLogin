using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AbpAzureAdLogin.Data;
using Volo.Abp.DependencyInjection;

namespace AbpAzureAdLogin.EntityFrameworkCore
{
    public class EntityFrameworkCoreAbpAzureAdLoginDbSchemaMigrator
        : IAbpAzureAdLoginDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreAbpAzureAdLoginDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the AbpAzureAdLoginMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<AbpAzureAdLoginMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}