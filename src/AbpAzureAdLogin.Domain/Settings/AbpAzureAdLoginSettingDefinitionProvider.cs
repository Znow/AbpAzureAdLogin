using Volo.Abp.Settings;

namespace AbpAzureAdLogin.Settings
{
    public class AbpAzureAdLoginSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(AbpAzureAdLoginSettings.MySetting1));
        }
    }
}
