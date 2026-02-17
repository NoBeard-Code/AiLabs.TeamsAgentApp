using AiLabs.TeamsBotApp.BusinessLogic;
using AiLabs.TeamsBotApp.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();

// SINGLE AGENT CONFIGURATION:

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

//var agents = builder.Configuration.GetSection("Agents").Get<List<AgentEntry>>()!;
//var agent = agents.FirstOrDefault() ?? throw new InvalidOperationException("No agents configured");

//builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp =>
//{
//    return AdapterFactory.CreateAdapter(agent.AppId, agent.ClientSecret, agent.TenantId);
//});

//builder.Services.AddTransient<IBot>(sp => new EndpointAgentBot(agent.Name));

// MULTI AGENT CONFIGURATION:

//builder.Services.Configure<AgentsOptions>(builder.Configuration.GetSection("Agents"));

var agents = builder.Configuration.GetSection("Agents").Get<List<AgentEntry>>()!;

foreach (var agent in agents)
{
    IBotFrameworkHttpAdapter keyedAdapter = AdapterFactory.CreateAdapter(agent.AppId, agent.ClientSecret, agent.TenantId, agent.AppType);
    builder.Services.AddKeyedSingleton<IBotFrameworkHttpAdapter>(agent.Name, keyedAdapter);

    IBot keyedBot = new EndpointAgentBot(agent.Name);
    builder.Services.AddKeyedTransient<IBot>(agent.Name, (sp, obj) => new EndpointAgentBot(agent.Name));
}

var app = builder.Build();

// Enable AspNet authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();

// SINGLE AGENT CONFIGURATION:

//var incomingRoute = app.MapPost($"/{agent.Name}/api/messages", 
//    async (HttpRequest request, HttpResponse response, IBotFrameworkHttpAdapter adapter, IBot bot) =>
//{
//    await adapter.ProcessAsync(request, response, bot);
//});

//if (!app.Environment.IsDevelopment())
//{
//    incomingRoute.RequireAuthorization();
//};

// MULTI AGENT CONFIGURATION:

// Map endpoints AFTER app is built
foreach (var agent in agents)
{
    var route = $"{agent.Name.ToLower()}/api/messages";

    var incomingRoute = app.MapPost(route,
        async (HttpRequest request, HttpResponse response) =>
        {
            var adapter = app.Services.GetRequiredKeyedService<IBotFrameworkHttpAdapter>(agent.Name);
            var bot = app.Services.GetRequiredKeyedService<IBot>(agent.Name);

            await adapter.ProcessAsync(request, response, bot);
        });

    if (!app.Environment.IsDevelopment())
    {
        incomingRoute.RequireAuthorization();
    }
}

if (app.Environment.IsDevelopment())
{
    // Hardcoded for brevity and ease of testing. 
    // In production, this should be set in configuration.
    app.Urls.Add($"http://localhost:3999");
}

app.Run();
