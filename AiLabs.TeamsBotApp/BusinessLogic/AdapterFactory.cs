using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace AiLabs.TeamsBotApp.BusinessLogic;

public static class AdapterFactory
{
    public static CloudAdapter CreateAdapter(string appId, string appPassword, string tenantId, ILogger logger = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MicrosoftAppType"] = "SingleTenant",
                ["MicrosoftAppId"] = appId,
                ["MicrosoftAppPassword"] = appPassword,
                ["MicrosoftAppTenantId"] = tenantId
            })
            .Build();
       
        var auth = new ConfigurationBotFrameworkAuthentication(config);

        return new CloudAdapter(auth, logger);
    }
}
