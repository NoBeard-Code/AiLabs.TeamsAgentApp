using Microsoft.Agents.Builder;

namespace AiLabs.TeamsAgentApp.BusinessLogic;

public interface IAgentRouter
{
    Task<string?> ResolveAsync(ITurnContext turnContext, CancellationToken cancellationToken);
}

public class AgentRouter : IAgentRouter
{
    public async Task<string?> ResolveAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        var tenantId = turnContext.Activity.Conversation.TenantId;
        var agentId = turnContext.Activity.Recipient?.Id; // Bot/App/Agent ID

        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(agentId))
            return null;

        // Normalize AppId
        var agentAppId = agentId.StartsWith("28:") ? agentId.Substring(3) : agentId;

        Console.WriteLine($"DEBUG: TenantId: {tenantId}");
        Console.WriteLine($"DEBUG: AgentAppId: {agentAppId}");

        return agentAppId;
    }
}
