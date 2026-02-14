using AiLabs.TeamsBotApp.BusinessLogic;
using AiLabs.TeamsBotApp.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();

#if SINGLE

/*
 
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp =>
{
    var auth = sp.GetRequiredService<BotFrameworkAuthentication>();
    var logger = sp.GetRequiredService<ILogger<CloudAdapter>>();
    return new CloudAdapter(auth, logger);
});

var routePrefix = "hrgeneralist";
builder.Services.AddTransient<IBot>(sp => new EndpointAgentBot(routePrefix));

*/

var agents = builder.Configuration.GetSection("Agents").Get<List<AgentEntry>>()!;
var agent = agents.FirstOrDefault() ?? throw new InvalidOperationException("No agents configured");

builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp =>
{
    return AdapterFactory.CreateAdapter(agent.AppId, agent.ClientSecret, agent.TenantId);
});

builder.Services.AddTransient<IBot>(sp => new EndpointAgentBot(agent.Name));

//builder.Services.AddSingleton<BotFrameworkAuthentication>(sp =>
//{
//    var config = sp.GetRequiredService<IConfiguration>();
//    return new ConfigurationBotFrameworkAuthentication(config);
//});

//builder.Services.AddSingleton<BotFrameworkAuthentication>(sp =>
//{
//    var config = sp.GetRequiredService<IConfiguration>();
//    var allowedCallers = config.GetSection("AllowedCallers").Get<string[]>();

//    var authConfig = new AuthenticationConfiguration
//    {
//        ClaimsValidator = new AllowedCallersClaimsValidator(allowedCallers),
//        ValidTokenIssuers = new[] { "https://api.botframework.com" }
//    };

//    return new ConfigurationBotFrameworkAuthentication(config, authConfiguration: authConfig);
//});

var app = builder.Build();

#elif MULTI

// Domain services
builder.Services.AddSingleton<IMultiAgentRouter, MultiAgentRouter>();
builder.Services.AddSingleton<IProcessExecutor, ProcessExecutor>();

//builder.Services.Configure<AgentsOptions>(builder.Configuration.GetSection("Agents"));

var agents = builder.Configuration.GetSection("Agents").Get<List<AgentEntry>>()!;

// Dictionaries to store per-agent services
//var adapterMap = new Dictionary<string, CloudAdapter>();
//var botFactoryMap = new Dictionary<string, Func<IBot>>();

foreach (var agent in agents)
{
    //builder.Services.AddTransient(typeof(EndpointAgentBot),
    //    sp => new EndpointAgentBot(agent.Name));

    //builder.Services.AddSingleton<Func<string, IBot>>(sp => { return agentName => new EndpointAgentBot(agentName); });
    //botFactoryMap[agent.Name] = () => new EndpointAgentBot(agent.Name);
}

var app = builder.Build();

// Register each agent's services
foreach (var agent in agents)
{
    var logger = app.Services.GetRequiredService<ILogger<CloudAdapter>>();

    var adapter = AdapterFactory.CreateAdapter(agent.AppId, agent.ClientSecret, agent.TenantId, logger);

    //var isolatedConfig = new ConfigurationBuilder()
    //    .AddInMemoryCollection(new Dictionary<string, string?>
    //    {
    //        ["MicrosoftAppId"] = agent.AppId,
    //        ["MicrosoftAppPassword"] = agent.ClientSecret
    //    })
    //    .Build();

    //var auth = new ConfigurationBotFrameworkAuthentication(isolatedConfig);

    //var logger = app.Services.GetRequiredService<ILogger<CloudAdapter>>();
    //var adapter = new CloudAdapter(auth, logger);

    adapterMap[agent.Name] = adapter;

    //botTypeMap[agent.Name] = typeof(EndpointAgentBot);     
}

#endif

// Enable AspNet authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();

#if SINGLE

var incomingRoute = app.MapPost($"/{agent.Name}/api/messages", 
    async (HttpRequest request, HttpResponse response, IBotFrameworkHttpAdapter adapter, IBot bot) =>
{
    await adapter.ProcessAsync(request, response, bot);
});

if (!app.Environment.IsDevelopment())
{
    incomingRoute.RequireAuthorization();
}
else
{
    // Hardcoded for brevity and ease of testing. 
    // In production, this should be set in configuration.
    app.Urls.Add($"http://localhost:3999");
}

#elif MULTI

// Map endpoints AFTER app is built
foreach (var agent in agents)
{
    var route = $"{agent.Name.ToLower()}/api/messages";

    //app.MapPost(route,
    //    async (HttpRequest req, HttpResponse res,
    //           CloudAdapter adapter, IBot bot) =>
    //    {
    //        await adapter.ProcessAsync(req, res, bot);
    //    });

    app.MapPost(route, async (HttpRequest req, HttpResponse res) =>
    {
        var adapter = adapterMap[agent.Name];
        //var botFactory = sp.GetRequiredService<Func<string, IBot>>(); 
        //var bot = botFactory(agent.Name);
        var bot = botFactoryMap[agent.Name]();

        await adapter.ProcessAsync(req, res, bot);
    });
}

#endif

app.Run();
