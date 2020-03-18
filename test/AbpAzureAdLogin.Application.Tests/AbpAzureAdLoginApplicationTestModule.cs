using Volo.Abp.Modularity;

namespace AbpAzureAdLogin
{
    [DependsOn(
        typeof(AbpAzureAdLoginApplicationModule),
        typeof(AbpAzureAdLoginDomainTestModule)
        )]
    public class AbpAzureAdLoginApplicationTestModule : AbpModule
    {

    }
}