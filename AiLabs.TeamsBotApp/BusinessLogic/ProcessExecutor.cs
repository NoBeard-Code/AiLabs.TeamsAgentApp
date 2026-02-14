using AiLabs.TeamsBotApp.Configuration;
using Microsoft.Bot.Builder;

namespace AiLabs.TeamsBotApp.BusinessLogic;

public interface IProcessExecutor
{
    Task ExecuteAsync(AgentEntry agent, ITurnContext turnContext, CancellationToken cancellationToken);
}

public class ProcessExecutor : IProcessExecutor
{
    public async Task ExecuteAsync(AgentEntry agent, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        var text = turnContext.Activity.Text ?? string.Empty;

        // Placeholder: here you can plug Teams.AI Application<TState> per agent.
        await turnContext.SendActivityAsync(
            MessageFactory.Text($"[{agent.Name}] received: '{text}' (tenant {agent.TenantId})"),
            cancellationToken);
    }
}
