using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace AbpAzureAdLogin.Pages
{
    public class Index_Tests : AbpAzureAdLoginWebTestBase
    {
        [Fact]
        public async Task Welcome_Page()
        {
            var response = await GetResponseAsStringAsync("/");
            response.ShouldNotBeNull();
        }
    }
}
