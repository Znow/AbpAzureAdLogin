using AbpAzureAdLogin.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace AbpAzureAdLogin.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpAzureAdLoginEntityFrameworkCoreDbMigrationsModule),
        typeof(AbpAzureAdLoginApplicationContractsModule)
        )]
    public class AbpAzureAdLoginDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
