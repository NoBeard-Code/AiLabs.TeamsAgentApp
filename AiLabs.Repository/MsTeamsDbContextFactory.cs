using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AiLabs.Repository;

public class MsTeamsDbContextFactory : IDesignTimeDbContextFactory<MsTeamsDbContext>
{
    public MsTeamsDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<MsTeamsDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer(string.Empty);

        return new MsTeamsDbContext(optionsBuilder.Options);
    }
}