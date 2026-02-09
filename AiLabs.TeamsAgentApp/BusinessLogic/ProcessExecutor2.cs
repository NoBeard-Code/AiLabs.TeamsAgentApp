using AiLabs.TeamsAgentApp.Configuration;
using Microsoft.Bot.Builder;

namespace AiLabs.TeamsAgentApp.BusinessLogic;

public interface IProcessExecutor2
{
    Task ExecuteAsync(AgentEntry agent, ITurnContext turnContext, CancellationToken cancellationToken);
}

public class ProcessExecutor2 : IProcessExecutor2
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
