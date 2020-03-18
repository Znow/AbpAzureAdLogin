using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace AbpAzureAdLogin.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(AbpAzureAdLoginHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class AbpAzureAdLoginConsoleApiClientModule : AbpModule
    {
        
    }
}
