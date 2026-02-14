using Microsoft.Agents.Builder;

namespace AiLabs.TeamsAgentApp.BusinessLogic;

public interface IProcessExecutor
{
    Task ExecuteAsync(string agentName, ITurnContext turnContext, CancellationToken cancellationToken);
}

public class ProcessExecutor : IProcessExecutor
{
    public async Task ExecuteAsync(string agentName, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        // Example: simple echo with agent name
        await turnContext.SendActivityAsync(
            $"[{agentName}] You said: {turnContext.Activity.Text}",
            cancellationToken: cancellationToken);
    }
}

