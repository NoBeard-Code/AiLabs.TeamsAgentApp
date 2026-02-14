using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using System.Diagnostics;

namespace AiLabs.TeamsAgentApp.BusinessLogic;

public class MyAgent : AgentApplication
{
    private readonly IAgentRouter _router; 
    private readonly IProcessExecutor _executor;

    public MyAgent(AgentApplicationOptions options, 
        IAgentRouter router, IProcessExecutor executor) : base(options)
    {
        _router = router; 
        _executor = executor;

        OnConversationUpdate(ConversationUpdateEvents.MembersAdded, WelcomeMessageAsync);
        OnActivity(ActivityTypes.Message, OnMessageAsync, rank: RouteRank.Last);
    }

    private async Task WelcomeMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        foreach (ChannelAccount member in turnContext.Activity.MembersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Hello and Welcome!"), cancellationToken);
            }
        }
    }

    private async Task OnMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        // 1. Resolve tenant + agent → process
        var agentId = await _router.ResolveAsync(turnContext, cancellationToken);

        if (agentId == null)
        {
           Console.WriteLine("No process configured for this agent in your tenant.");
            return;
        }

        // 2. Execute the process
        await _executor.ExecuteAsync(agentId, turnContext, cancellationToken);

        //await turnContext.SendActivityAsync($"You said: {turnContext.Activity.Text}", cancellationToken: cancellationToken);
    }
}