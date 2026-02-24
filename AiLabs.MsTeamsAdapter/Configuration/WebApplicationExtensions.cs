using AiLabs.Repository;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams;
using Microsoft.EntityFrameworkCore;

namespace AiLabs.MsTeamsAdapter.Configuration;

public static class WebApplicationExtensions
{
    public static async Task InitializeDynamicBotServicesAsync(this WebApplication app)
    {
        var adapterRegistry = app.Services.GetRequiredService<IKeyedServiceRegistry<IBotFrameworkHttpAdapter>>();
        var botRegistry = app.Services.GetRequiredService<IKeyedServiceRegistry<TeamsActivityHandler>>();

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MsTeamsDbContext>();

        var agents = await context.Agents
            .Include(x => x.Registration)
            .ToListAsync();

        foreach (var agent in agents)
        {
            // Create adapter dynamically
            var adapter = AdapterFactory.CreateAdapter(
                agent.Registration.ClientId,
                agent.Registration.ClientSecret,
                agent.Registration.TenantId,                
                agent.Registration.AppType);

            adapterRegistry.Add(agent.AppId, adapter);

            // Create bot dynamically
            var bot = new EndpointAgentBot(agent.AppId);
            botRegistry.Add(agent.AppId, bot);
        }

        foreach (var agent in agents)
        {
            var route = $"{agent.AppId}/api/messages";

            var incomingRoute = app.MapPost(route,
                async (HttpRequest request, HttpResponse response) =>
                {
                    if (!adapterRegistry.TryGet(agent.AppId, out var adapter))
                        throw new InvalidOperationException($"Adapter not found for agent '{agent.ShortName}'");

                    if (!botRegistry.TryGet(agent.AppId, out var bot))
                        throw new InvalidOperationException($"Bot not found for agent '{agent.ShortName}'");

                    await adapter.ProcessAsync(request, response, bot);
                });

            if (!app.Environment.IsDevelopment())
            {
                incomingRoute.RequireAuthorization();
            }
        }
    }
}
