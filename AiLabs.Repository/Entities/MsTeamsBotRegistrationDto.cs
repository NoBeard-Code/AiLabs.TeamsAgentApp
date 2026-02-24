using System.ComponentModel.DataAnnotations;

namespace AiLabs.Repository.Entities;

public class MsTeamsBotRegistrationDto
{
    [Key]
    public int MsTeamsBotRegistrationId { get; set; }

    [Required]
    public Guid BotId { get; set; }

    [Required, Url]
    public string EndpointAddress { get; set; } = null!;

    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public string ClientSecret { get; set; } = null!;

    [Required]
    public string AppType { get; set; } = "MultiTenant"; // SingleTenant or MultiTenant
}
