using AbpAzureAdLogin.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace AbpAzureAdLogin
{
    [DependsOn(
        typeof(AbpAzureAdLoginEntityFrameworkCoreTestModule)
        )]
    public class AbpAzureAdLoginDomainTestModule : AbpModule
    {

    }
}