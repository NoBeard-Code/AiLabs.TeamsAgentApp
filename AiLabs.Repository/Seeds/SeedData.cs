using AiLabs.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiLabs.Repository.Seeds;

public class SeedData
{
    public static async Task CreateAgents(MsTeamsDbContext context)
    {
        if (await context.Agents.AnyAsync())
        {
            return;
        }

        var developer = new MsTeamsDeveloperDto
        {
            Name = "GDW Innovations",
            WebsiteUrl = "https://www.gdwinnovations.com",
            PrivacyUrl = "https://www.gdwinnovations.com/privacy",
            TermsOfUseUrl = "https://www.gdwinnovations.com/terms"
        };

        await context.Developers.AddAsync(developer);

        var validDomain = "https://36m8t70c-3999.euw.devtunnels.ms";
        var tenantId = Guid.Parse("9f08d951-52d4-4953-9b47-44072988d50a");

        await AddAgentAsync(context, developer, validDomain, tenantId, "f55b52bc-d987-41ea-9808-3c077213c6db", "DefComm Dev - HR Generalist",
            "1f68832a-05c0-4725-9f77-a7d590758a3a", "qCo8Q~kqV_BRgvTf8M.xNcwBj4GcUY0MtcJ5DdeD", "SingleTenant");

        await AddAgentAsync(context, developer, validDomain, tenantId, "adf94c44-f42b-4971-b18e-5d4010ae5e86", "DefComm Dev - Accountant",
            "1d280068-1b03-4590-b5c0-8b36c686bdc5", "mt48Q~F7LlGgCj.JSzgm8EpuXTB0rHQWTCbI9arU", "SingleTenant");

        await AddAgentAsync(context, developer, validDomain, tenantId, "42636a5d-86c6-4ca5-8e95-1c6cb2f078ea", "DefComm Dev - Developer",
            "b89bcb3f-6775-4a31-94f8-4f0c624071e9", "h4~8Q~bI2Qnf-NmXHFKjBBq6VbADcz2uuvb-daOg", "MultiTenant");

        await AddAgentAsync(context, developer, validDomain, tenantId, "8dbc8569-66fc-4944-bec1-0173b8843b6a", "DefComm Dev - Tester",
            "73ca7801-2f33-48ea-9c6d-f84f5e7b2a01", "iG18Q~YaGGqIho8V_7Q7PT6iOQ~klWJf5cpXlajQ", "MultiTenant");

        await AddAgentAsync(context, developer, validDomain, tenantId, "10bca4e9-35f9-4921-b8bd-94bd802363cb", "DefComm Dev - DevOps Engineer",
            "f51a2bfc-3fcd-4f7a-9ad8-432f2572a6f1", "iKW8Q~sobt7O~9EyQA0p6qcA9LY11z5jblj-4dgf", "MultiTenant");

        await context.SaveChangesAsync();
    }

    private static async Task AddAgentAsync(MsTeamsDbContext context, MsTeamsDeveloperDto developer, string validDomain, Guid tenantId,
        string appId, string name, string botId, string clientSecret, string appType)
    {
        var botGuid = Guid.Parse(botId);

        await context.Agents.AddAsync(new MsTeamsBotDto
        {
            AppId = Guid.Parse(appId),
            ShortName = name,
            Developer = developer,
            ValidDomain = validDomain,
            Registration = new MsTeamsBotRegistrationDto
            {
                BotId = botGuid,
                EndpointAddress = $"{validDomain}/{appId}/api/messages",
                TenantId = tenantId,
                ClientId = botGuid,
                ClientSecret = clientSecret,
                AppType = appType
            }
        });
    }
}
