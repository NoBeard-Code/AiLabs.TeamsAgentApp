using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace AiLabs.TeamsAgentApp.BusinessLogic;

public class GenericAgentBot : IBot
{
    private readonly IMultiAgentRouter _router;
    private readonly IProcessExecutor2 _executor;

    public GenericAgentBot(IMultiAgentRouter router, IProcessExecutor2 executor)
    {
        _router = router;
        _executor = executor;
    }

    public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
        if (turnContext.Activity.Type != ActivityTypes.Message)
            return;

        var message = turnContext.Activity.AsMessageActivity();

        var tenantId =
            turnContext.Activity.Conversation?.TenantId ??
            turnContext.Activity.GetChannelData<dynamic>()?.tenant?.id ??
            string.Empty;

        var rawBotId = turnContext.Activity.Recipient?.Id ?? string.Empty; // "28:<appId>"
        var appId = rawBotId.StartsWith("28:") ? rawBotId[3..] : rawBotId;

        var agent = _router.GetAgent(tenantId, appId);

        await _executor.ExecuteAsync(agent, turnContext, cancellationToken);
    }
}
