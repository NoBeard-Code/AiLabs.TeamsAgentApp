using AiLabs.TeamsAgentApp.BusinessLogic;
using AiLabs.TeamsAgentApp.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

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

#elif TEAMS

builder.Services.Configure<AgentsOptions>(builder.Configuration.GetSection("Agents"));

// Domain services
builder.Services.AddSingleton<IMultiAgentRouter, MultiAgentRouter>(); 
builder.Services.AddSingleton<IProcessExecutor2, ProcessExecutor2>();

// Bot Framework auth (Azure Bot Service)
builder.Services.AddSingleton<BotFrameworkAuthentication>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new ConfigurationBotFrameworkAuthentication(config);
});

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
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp =>
{
    var auth = sp.GetRequiredService<BotFrameworkAuthentication>();
    var logger = sp.GetRequiredService<ILogger<CloudAdapter>>();
    return new CloudAdapter(auth, logger);
});

// Bot
builder.Services.AddTransient<IBot, GenericAgentBot>();

#endif

builder.Services.AddControllers();

var app = builder.Build();

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

var incomingRoute = app.MapPost("/api/messages", async (HttpRequest request, HttpResponse response, IBotFrameworkHttpAdapter adapter, IBot bot) => 
{ 
    await adapter.ProcessAsync(request, response, bot); 
});

#endif

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

app.Run();
