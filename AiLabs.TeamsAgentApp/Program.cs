using AiLabs.TeamsAgentApp.BusinessLogic;
using AiLabs.TeamsAgentApp.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();

#if AGENTS

using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;

// Add AgentApplicationOptions from appsettings section "AgentApplication".
builder.AddAgentApplicationOptions();

// Domain services
builder.Services.AddSingleton<IAgentRouter, AgentRouter>();
builder.Services.AddSingleton<IProcessRepository, ProcessRepository>();
builder.Services.AddSingleton<IProcessExecutor, ProcessExecutor>();

// Add the AgentApplication, which contains the logic for responding to user messages.
builder.AddAgent<MyAgent>();

// Add AspNet token validation for Azure Bot Service and Entra.  Authentication is
// configured in the appsettings.json "TokenValidation" section.
builder.Services.AddAgentAspNetAuthentication(builder.Configuration);

// Register IStorage.  For development, MemoryStorage is suitable.
// For production Agents, persisted storage should be used so
// that state survives Agent restarts, and operates correctly
// in a cluster of Agent instances.
builder.Services.AddSingleton<IStorage, MemoryStorage>();

var app = builder.Build();

#elif TEAMS

//builder.Services.Configure<AgentsOptions>(builder.Configuration.GetSection("Agents"));

#region single agent 

var agents = builder.Configuration.GetSection("Agents").Get<List<AgentEntry>>()!;

// Dictionaries to store per-agent services
var adapterMap = new Dictionary<string, CloudAdapter>();
var botFactoryMap = new Dictionary<string, Func<IBot>>();

// ✔ Register bots BEFORE builder.Build()
foreach (var agent in agents)
{
    //builder.Services.AddTransient(typeof(EndpointAgentBot),
    //    sp => new EndpointAgentBot(agent.Name));

    //builder.Services.AddSingleton<Func<string, IBot>>(sp => { return agentName => new EndpointAgentBot(agentName); });
    botFactoryMap[agent.Name] = () => new EndpointAgentBot(agent.Name);
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

// Domain services
//builder.Services.AddSingleton<IMultiAgentRouter, MultiAgentRouter>(); 
//builder.Services.AddSingleton<IProcessExecutor2, ProcessExecutor2>();

// Bot Framework auth (Azure Bot Service)
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

// Adapter
//builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp =>
//{
//    var auth = sp.GetRequiredService<BotFrameworkAuthentication>();
//    var logger = sp.GetRequiredService<ILogger<CloudAdapter>>();
//    return new CloudAdapter(auth, logger);
//});

// Bot
//builder.Services.AddTransient<IBot, GenericAgentBot>();

#endregion

#endif


// Enable AspNet authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "GDW Innovations - DefComm Dev Agent");

#if AGENTS

// This receives incoming messages from Azure Bot Service or other SDK Agents
var incomingRoute = app.MapPost("/api/messages", 
    async (HttpRequest request, HttpResponse response, IAgentHttpAdapter adapter, IAgent agent, CancellationToken cancellationToken) =>
{
    await adapter.ProcessAsync(request, response, agent, cancellationToken);
});

#elif TEAMS

#region single route for multiple agents

//var incomingRoute = app.MapPost("/api/messages", async (HttpRequest request, HttpResponse response, IBotFrameworkHttpAdapter adapter, IBot bot) => 
//{ 
//    await adapter.ProcessAsync(request, response, bot); 
//});

#endregion

// Map endpoints AFTER app is built
foreach (var agent in agents)
{
    var route = $"/api/messages/{agent.Name.ToLower()}";

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

if (!app.Environment.IsDevelopment())
{
    //incomingRoute.RequireAuthorization();
}
else
{
    // Hardcoded for brevity and ease of testing. 
    // In production, this should be set in configuration.
    app.Urls.Add($"http://localhost:3999");
}

app.Run();
