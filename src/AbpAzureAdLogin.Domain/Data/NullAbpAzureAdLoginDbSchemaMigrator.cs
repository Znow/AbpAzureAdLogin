using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AbpAzureAdLogin.Data
{
    /* This is used if database provider does't define
     * IAbpAzureAdLoginDbSchemaMigrator implementation.
     */
    public class NullAbpAzureAdLoginDbSchemaMigrator : IAbpAzureAdLoginDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}