using AiLabs.TeamsAgentApp.Configuration;
using Microsoft.Extensions.Options;

namespace AiLabs.TeamsAgentApp.BusinessLogic;

public interface IMultiAgentRouter
{
    AgentEntry GetAgent(string tenantId, string appId);
}

public class MultiAgentRouter : IMultiAgentRouter
{
    private readonly AgentsOptions _options;

    public MultiAgentRouter(IOptions<AgentsOptions> options)
    {
        _options = options.Value;
    }

    public AgentEntry GetAgent(string tenantId, string appId)
    {
        return _options.Agents.FirstOrDefault(a =>
            string.Equals(a.TenantId, tenantId, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(a.AppId, appId, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(
                $"No agent found for tenant '{tenantId}' and app '{appId}'.");
    }
}
