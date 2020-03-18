using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace AbpAzureAdLogin.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpAzureAdLoginEntityFrameworkCoreModule)
        )]
    public class AbpAzureAdLoginEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpAzureAdLoginMigrationsDbContext>();
        }
    }
}
