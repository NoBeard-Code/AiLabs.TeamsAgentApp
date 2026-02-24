using AiLabs.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AiLabs.Repository;

public class MsTeamsDbContext : DbContext
{
    public DbSet<MsTeamsBotDto> Agents { get; set; }
    public DbSet<MsTeamsDeveloperDto> Developers { get; set; }
    public DbSet<MsTeamsBotRegistrationDto> Bots { get; set; }

    public MsTeamsDbContext(DbContextOptions<MsTeamsDbContext> options): base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
