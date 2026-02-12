namespace AiLabs.TeamsAgentApp.BusinessLogic;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

public class EndpointAgentBot : IBot
{
    private readonly string _agentName;

    public EndpointAgentBot(string agentName)
    {
        _agentName = agentName;
    }

    public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        if (turnContext.Activity.Type != ActivityTypes.Message)
            return;

        var tenantId =
            turnContext.Activity.Conversation?.TenantId ??
            turnContext.Activity.GetChannelData<dynamic>()?.tenant?.id ??
            string.Empty;

        var rawBotId = turnContext.Activity.Recipient?.Id ?? string.Empty; // "28:<appId>"
        var appId = rawBotId.StartsWith("28:") ? rawBotId[3..] : rawBotId;

        var text = turnContext.Activity.Text ?? string.Empty;

        Console.WriteLine($"[{_agentName}] TenantId: {tenantId}");
        Console.WriteLine($"[{_agentName}] AppId: {appId}");
        Console.WriteLine($"[{_agentName}] Received message: {text}");

        await turnContext.SendActivityAsync(
            MessageFactory.Text($"[{_agentName}] received: {text}"),
            cancellationToken);
    }
}
