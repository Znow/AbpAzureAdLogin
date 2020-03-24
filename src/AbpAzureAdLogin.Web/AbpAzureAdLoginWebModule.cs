﻿using System;
using System.IO;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AbpAzureAdLogin.EntityFrameworkCore;
using AbpAzureAdLogin.Localization;
using AbpAzureAdLogin.MultiTenancy;
using AbpAzureAdLogin.Web.Menus;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using IdentityServer4;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;

namespace AbpAzureAdLogin.Web
{
    [DependsOn(
        typeof(AbpAzureAdLoginHttpApiModule),
        typeof(AbpAzureAdLoginApplicationModule),
        typeof(AbpAzureAdLoginEntityFrameworkCoreDbMigrationsModule),
        typeof(AbpAutofacModule),
        typeof(AbpIdentityWebModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpTenantManagementWebModule),
        typeof(AbpAspNetCoreSerilogModule)
        )]
    public class AbpAzureAdLoginWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(
                    typeof(AbpAzureAdLoginResource),
                    typeof(AbpAzureAdLoginDomainModule).Assembly,
                    typeof(AbpAzureAdLoginDomainSharedModule).Assembly,
                    typeof(AbpAzureAdLoginApplicationModule).Assembly,
                    typeof(AbpAzureAdLoginApplicationContractsModule).Assembly,
                    typeof(AbpAzureAdLoginWebModule).Assembly
                );
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services
                .GetObject<IdentityBuilder>()
                .AddSignInManager<CustomSigninManager>();

            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            ConfigureUrls(configuration);
            ConfigureAuthentication(context, configuration);
            ConfigureAutoMapper();
            ConfigureVirtualFileSystem(hostingEnvironment);
            ConfigureLocalizationServices();
            ConfigureNavigationServices();
            ConfigureAutoApiControllers();
            ConfigureSwaggerServices(context.Services);
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("http://schemas.microsoft.com/identity/claims/objectidentifier", ClaimTypes.NameIdentifier);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("oid", ClaimTypes.NameIdentifier);
            context.Services.AddAuthentication()
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "AbpAzureAdLogin";
                });

            //context.Services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
            context.Services.AddAuthentication(IdentityConstants.ExternalScheme)
                .AddAzureAD(options => configuration.Bind("AzureAd", options));

            context.Services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                //options.Authority = options.Authority + "/v2.0/";         // Microsoft identity platform
                options.Authority = "https://login.microsoftonline.com/" + configuration["AzureAd:TenantId"];

                options.TokenValidationParameters.ValidateIssuer = false; // accept several tenants (here simplified)

                options.GetClaimsFromUserInfoEndpoint = true;
                options.SignInScheme = IdentityConstants.ExternalScheme;
                //options.SignOutScheme = IdentityConstants.ApplicationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.ForwardSignIn = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.Events.OnTokenValidated = (async context =>
                {
                    var dodo = context.Principal.Identity;

                    await Task.CompletedTask;
                });
            });
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AbpAzureAdLoginWebModule>();

            });
        }

        private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<AbpAzureAdLoginDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpAzureAdLogin.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<AbpAzureAdLoginDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpAzureAdLogin.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<AbpAzureAdLoginApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpAzureAdLogin.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<AbpAzureAdLoginApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpAzureAdLogin.Application"));
                    options.FileSets.ReplaceEmbeddedByPhysical<AbpAzureAdLoginWebModule>(hostingEnvironment.ContentRootPath);
                });
            }
        }

        private void ConfigureLocalizationServices()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<AbpAzureAdLoginResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );

                options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            });
        }

        private void ConfigureNavigationServices()
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new AbpAzureAdLoginMenuContributor());
            });
        }

        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(AbpAzureAdLoginApplicationModule).Assembly);
            });
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AbpAzureAdLogin API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            app.UseCorrelationId();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseErrorPage();
            }
            app.UseVirtualFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseAbpRequestLocalization();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "AbpAzureAdLogin API");
            });
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseMvcWithDefaultRouteAndArea();
        }

        private void ConfigureAuthenticationWithOldADConfig(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication()
                .AddOpenIdConnect("aad", "Azure AD", options =>
                {
                    //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    //options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    //options.SignOutScheme = IdentityConstants.SignoutScheme;

                    options.Authority = "https://login.microsoftonline.com/" + configuration["AzureAd:TenantId"];
                    options.ClientId = configuration["AzureAd:ClientId"];
                    options.ClientSecret = configuration["AzureAd:ClientId"];
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.CallbackPath = "/signin-oidc";
                    //options.SignedOutCallbackPath = "/signout-callback-aad";
                    //options.RemoteSignOutPath = "/signout-aad";
                    options.SignedOutRedirectUri = "/signout-oidc";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                    options.Events.OnTokenValidated = (async context =>
                    {
                        var dodo = context.Principal.Identity;
                        await Task.CompletedTask;
                    });

                    options.GetClaimsFromUserInfoEndpoint = true;
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("sub", ClaimTypes.NameIdentifier);
                })
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "AbpAzureAdLogin";
                });
        }
    }
}
