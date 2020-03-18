using AbpAzureAdLogin.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace AbpAzureAdLogin.Permissions
{
    public class AbpAzureAdLoginPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(AbpAzureAdLoginPermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(AbpAzureAdLoginPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpAzureAdLoginResource>(name);
        }
    }
}
