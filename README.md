# AbpAzureAdLogin

1. Clone project
2. Setup DB and run migrations if needed, or use existing DB with ABP tables
3. Setup Azure App Registration, and configure in appsettings with correct values
```
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxx",
    "ClientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxx",
    "Domain": "domain.onmicrosoft.com",
    "CallbackPath": "/signin-oidc",
    "ClientSecret": "somesecrethere"
  },
  "App": {
    "SelfUrl": "https://localhost:44307"
  },
  "ConnectionStrings": {
    "Default": "Server=(LocalDb)\\MSSQLLocalDB;Database=AbpAzureAdLogin;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "AuthServer": {
    "Authority": "https://localhost:44307"
  },
  "IdentityServer": {
    "Clients": {
      "AbpAzureAdLogin_App": {
        "ClientId": "AbpAzureAdLogin_App"
      }
    }
  }
}
```
4. Run project and click on "Login"-button or go to "/Account/Login"
5. You should see a page matching this below, with "Azure Active Directory"-button
![image](https://user-images.githubusercontent.com/265719/76947251-a27bd980-6905-11ea-8db2-8c8fed568406.png)
6. Click on above mentioned button, and signin with your Azure AD credentials.
7. User is created, but not logged in due to incorrect scheme
