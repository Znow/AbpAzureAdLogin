using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AbpAzureAdLogin.Web
{
    public class CustomSigninManager : SignInManager<Volo.Abp.Identity.IdentityUser>
    {
        private const string LoginProviderKey = "LoginProvider";
        private const string XsrfKey = "XsrfId";
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        

        public CustomSigninManager(
            UserManager<Volo.Abp.Identity.IdentityUser> userManager,
            Microsoft.AspNetCore.Http.IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser> claimsFactory,
            Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor,
            Microsoft.Extensions.Logging.ILogger<SignInManager<Volo.Abp.Identity.IdentityUser>> logger,
            Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider schemes,
            IUserConfirmation<Volo.Abp.Identity.IdentityUser> confirmation,
            IAuthenticationSchemeProvider authenticationSchemeProvider) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }
        public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            var user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            var error = await PreSignInCheck(user);
            if (error != null)
            {
                return error;
            }
            return await SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        }

        public override async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            // Default AuthenticationScheme
            AuthenticationScheme defaultScheme = await _authenticationSchemeProvider.GetDefaultAuthenticateSchemeAsync();
            // The scheme that will be used by default
            AuthenticationScheme defaultSignoutScheme = await _authenticationSchemeProvider.GetDefaultSignOutSchemeAsync();
            // Returns all schemes currently registered 
            IEnumerable<AuthenticationScheme> listAuthenticationSchemeProvider = await _authenticationSchemeProvider.GetAllSchemesAsync();

            var auth = await Context.AuthenticateAsync(IdentityConstants.ExternalScheme);
            //var auth = await Context.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var items = auth?.Properties?.Items;
            if (auth?.Principal == null || items == null || !items.ContainsKey(LoginProviderKey))
            {
                return null;
            }

            if (expectedXsrf != null)
            {
                if (!items.ContainsKey(XsrfKey))
                {
                    return null;
                }
                var userId = items[XsrfKey] as string;
                if (userId != expectedXsrf)
                {
                    return null;
                }
            }

            var providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = items[LoginProviderKey] as string;
            if (providerKey == null || provider == null)
            {
                return null;
            }

            var providerDisplayName = (await GetExternalAuthenticationSchemesAsync()).FirstOrDefault(p => p.Name == provider)?.DisplayName
                                      ?? provider;
            return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
            {
                AuthenticationTokens = auth.Properties.GetTokens()
            };
        }

        protected override async Task<SignInResult> SignInOrTwoFactorAsync(Volo.Abp.Identity.IdentityUser user, bool isPersistent, string loginProvider = null, bool bypassTwoFactor = false)
        {
            if (!bypassTwoFactor && await IsTfaEnabled(user))
            {
                if (!await IsTwoFactorClientRememberedAsync(user))
                {
                    // Store the userId for use after two factor check
                    var userId = await UserManager.GetUserIdAsync(user);
                    await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(userId, loginProvider));
                    return SignInResult.TwoFactorRequired;
                }
            }
            // Cleanup external cookie
            if (loginProvider != null)
            {
                await Context.SignOutAsync(IdentityConstants.ExternalScheme);
            }
            await SignInAsync(user, isPersistent, loginProvider);
            return SignInResult.Success;
        }

        public override async Task SignInAsync(Volo.Abp.Identity.IdentityUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            var userPrincipal = await CreateUserPrincipalAsync(user);
            // Review: should we guard against CreateUserPrincipal returning null?
            if (authenticationMethod != null)
            {
                userPrincipal.Identities.First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            }
            await Context.SignInAsync(IdentityConstants.ApplicationScheme,
                userPrincipal,
                authenticationProperties ?? new AuthenticationProperties());
        }
        private async Task<bool> IsTfaEnabled(Volo.Abp.Identity.IdentityUser user)
            => UserManager.SupportsUserTwoFactor &&
            await UserManager.GetTwoFactorEnabledAsync(user) &&
            (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

        internal ClaimsPrincipal StoreTwoFactorInfo(string userId, string loginProvider)
        {
            var identity = new ClaimsIdentity(IdentityConstants.TwoFactorUserIdScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, userId));
            if (loginProvider != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
            }
            return new ClaimsPrincipal(identity);
        }
    }
}
