using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace AiLabs.MsTeamsAdapter.Configuration;

public class EndpointAgentBot(Guid agentId) : TeamsActivityHandler
{
    protected override Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnInvokeActivityAsync(turnContext, cancellationToken);
    }

    protected override Task OnTeamsChannelRestoredAsync(ChannelInfo channelInfo, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnTeamsChannelRestoredAsync(channelInfo, teamInfo, turnContext, cancellationToken);
    }

    protected override Task OnSignInInvokeAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnSignInInvokeAsync(turnContext, cancellationToken);
    }

    protected override Task OnCommandActivityAsync(ITurnContext<ICommandActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnCommandActivityAsync(turnContext, cancellationToken);
    }

    protected override Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnEventActivityAsync(turnContext, cancellationToken);
    }

    protected override Task OnEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnEventAsync(turnContext, cancellationToken);
    }

    protected override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnMessageActivityAsync(turnContext, cancellationToken);
    }

    protected override Task OnInstallationUpdateActivityAsync(ITurnContext<IInstallationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnInstallationUpdateActivityAsync(turnContext, cancellationToken);
    }

    protected override Task<InvokeResponse> OnTeamsCardActionInvokeAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnTeamsCardActionInvokeAsync(turnContext, cancellationToken);
    }

    protected override Task<ConfigResponseBase> OnTeamsConfigFetchAsync(ITurnContext<IInvokeActivity> turnContext, JObject configData, CancellationToken cancellationToken)
    {
        return base.OnTeamsConfigFetchAsync(turnContext, configData, cancellationToken);
    }

    protected override Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
    {
        return base.OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
    }

    protected override Task OnTokenResponseEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
    {
        return base.OnTokenResponseEventAsync(turnContext, cancellationToken);
    }

    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        if (turnContext.Activity.Type != ActivityTypes.Message)
            return;

        var channelData = turnContext.Activity.GetChannelData<dynamic>();

        var tenantId =
            turnContext.Activity.Conversation?.TenantId ??
            turnContext.Activity.GetChannelData<dynamic>()?.tenant?.id ??
            string.Empty;

        var rawBotId = turnContext.Activity.Recipient?.Id ?? string.Empty; // "28:<appId>"
        var appId = rawBotId.StartsWith("28:") ? rawBotId[3..] : rawBotId;

        var text = turnContext.Activity.Text ?? string.Empty;

        Console.WriteLine($"[{agentId}] TenantId: {tenantId}");
        Console.WriteLine($"[{agentId}] AppId: {appId}");
        Console.WriteLine($"[{agentId}] Received message: {text}");

        await turnContext.SendActivityAsync(
            MessageFactory.Text($"[{agentId}] received: {text}"),
            cancellationToken);
    }
}
