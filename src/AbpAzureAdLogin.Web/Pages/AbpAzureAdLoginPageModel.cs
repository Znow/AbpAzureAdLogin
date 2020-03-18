using AbpAzureAdLogin.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace AbpAzureAdLogin.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class AbpAzureAdLoginPageModel : AbpPageModel
    {
        protected AbpAzureAdLoginPageModel()
        {
            LocalizationResourceType = typeof(AbpAzureAdLoginResource);
        }
    }
}