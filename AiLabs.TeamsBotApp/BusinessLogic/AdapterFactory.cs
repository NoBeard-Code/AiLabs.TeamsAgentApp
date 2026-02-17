using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace AiLabs.TeamsBotApp.BusinessLogic;

public static class AdapterFactory
{
    public static CloudAdapter CreateAdapter(string appId, string appPassword, string tenantId, string appType, ILogger logger = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MicrosoftAppId"] = appId,
                ["MicrosoftAppPassword"] = appPassword,
                ["MicrosoftAppTenantId"] = tenantId,
                ["MicrosoftAppType"] = appType
            })
            .Build();
       
        var auth = new ConfigurationBotFrameworkAuthentication(config);

        return new CloudAdapter(auth, logger);
    }
}
