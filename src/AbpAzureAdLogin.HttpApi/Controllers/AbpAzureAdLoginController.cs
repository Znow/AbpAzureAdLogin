using AbpAzureAdLogin.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AbpAzureAdLogin.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class AbpAzureAdLoginController : AbpController
    {
        protected AbpAzureAdLoginController()
        {
            LocalizationResource = typeof(AbpAzureAdLoginResource);
        }
    }
}