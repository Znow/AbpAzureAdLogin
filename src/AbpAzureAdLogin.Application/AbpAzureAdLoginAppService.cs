using System;
using System.Collections.Generic;
using System.Text;
using AbpAzureAdLogin.Localization;
using Volo.Abp.Application.Services;

namespace AbpAzureAdLogin
{
    /* Inherit your application services from this class.
     */
    public abstract class AbpAzureAdLoginAppService : ApplicationService
    {
        protected AbpAzureAdLoginAppService()
        {
            LocalizationResource = typeof(AbpAzureAdLoginResource);
        }
    }
}
