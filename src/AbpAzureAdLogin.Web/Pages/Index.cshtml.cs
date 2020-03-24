using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AbpAzureAdLogin.Web.Pages
{
    public class IndexModel : AbpAzureAdLoginPageModel
    {
        private readonly IAuthorizationService _authService;
        private readonly IAuthenticationService _authenticateService;

        public List<Claim> UserClaimList = new List<Claim>();

        public IndexModel(IAuthorizationService authService, IAuthenticationService authenticateService)
        {
            _authService = authService;
            _authenticateService = authenticateService;
        }

        public void OnGet()
        {
            var user = CurrentUser;
        }
    }
}