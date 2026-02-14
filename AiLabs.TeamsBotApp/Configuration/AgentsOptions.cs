namespace AiLabs.TeamsBotApp.Configuration;

public class AgentsOptions
{
    public List<AgentEntry> Agents { get; set; } = [];
}

public class AgentEntry
{
    public string Name { get; set; } = default!;
    public string AppId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string TenantId { get; set; } = default!;
}
