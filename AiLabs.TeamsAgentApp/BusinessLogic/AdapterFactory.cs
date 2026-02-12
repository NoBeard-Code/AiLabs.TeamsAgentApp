using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;

namespace AiLabs.TeamsAgentApp.BusinessLogic;

public static class AdapterFactory
{
    public static CloudAdapter CreateAdapter(string appId, string appPassword, string tenantId, ILogger logger)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MicrosoftAppId"] = appId,
                ["MicrosoftAppPassword"] = appPassword,
                ["MicrosoftAppType"] = "MultiTenant",
                ["MicrosoftAppTenantId"] = tenantId
            })
            .Build();

        //string[] allowedCallers = [ appId ];

        //var authConfig = new AuthenticationConfiguration
        //{
        //    ClaimsValidator = new AllowedCallersClaimsValidator(allowedCallers),
        //    ValidTokenIssuers = new[] { "https://api.botframework.com" }
        //};

        //var credentalsFactory = new ConfigurationServiceClientCredentialFactory(config);
        //credentalsFactory.CreateCredentialsAsync(appId, tenantId, "https://login.microsoftonline.com", false, new CancellationToken()).Wait();

        //var auth = new ConfigurationBotFrameworkAuthentication(config, authConfiguration: authConfig, credentialsFactory: credentalsFactory);
        
        var auth = new ConfigurationBotFrameworkAuthentication(config);

        return new CloudAdapter(auth, logger);
    }
}
