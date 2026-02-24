using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace AiLabs.MsTeamsAdapter.Configuration;

public static class AdapterFactory
{
    public static CloudAdapter CreateAdapter(Guid appId, string appPassword, Guid tenantId, string appType, ILogger logger = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MicrosoftAppId"] = appId.ToString(),
                ["MicrosoftAppPassword"] = appPassword,
                ["MicrosoftAppTenantId"] = tenantId.ToString(),
                ["MicrosoftAppType"] = appType
            })
            .Build();
       
        var auth = new ConfigurationBotFrameworkAuthentication(config);

        return new CloudAdapter(auth, logger);
    }
}
