using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using AbpAzureAdLogin.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace AbpAzureAdLogin.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits AbpAzureAdLogin.Web.Pages.AbpAzureAdLoginPage
     */
    public abstract class AbpAzureAdLoginPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<AbpAzureAdLoginResource> L { get; set; }
    }
}
