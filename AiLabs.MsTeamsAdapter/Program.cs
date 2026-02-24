using AiLabs.MsTeamsAdapter.Configuration;
using AiLabs.Repository;
using AiLabs.Repository.Seeds;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddDbContext<MsTeamsDbContext>(options =>
    options.UseInMemoryDatabase("MsTeamsDb"));

builder.Services.AddSingleton<IKeyedServiceRegistry<IBotFrameworkHttpAdapter>, KeyedServiceRegistry<IBotFrameworkHttpAdapter>>();
builder.Services.AddSingleton<IKeyedServiceRegistry<TeamsActivityHandler>, KeyedServiceRegistry<TeamsActivityHandler>>();

var app = builder.Build();

// Enable AspNet authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    // Hardcoded for brevity and ease of testing. 
    // In production, this should be set in configuration.
    app.Urls.Add($"http://localhost:3999");
}

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<MsTeamsDbContext>();
await SeedData.CreateAgents(context);

await app.InitializeDynamicBotServicesAsync();

app.Run();

