using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace AbpAzureAdLogin.EntityFrameworkCore
{
    public static class AbpAzureAdLoginDbContextModelCreatingExtensions
    {
        public static void ConfigureAbpAzureAdLogin(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(AbpAzureAdLoginConsts.DbTablePrefix + "YourEntities", AbpAzureAdLoginConsts.DbSchema);

            //    //...
            //});
        }
    }
}