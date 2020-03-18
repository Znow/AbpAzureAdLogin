using System.Threading.Tasks;

namespace AbpAzureAdLogin.Data
{
    public interface IAbpAzureAdLoginDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
