using System.ComponentModel.DataAnnotations;

namespace AiLabs.Repository.Entities;

// https://learn.microsoft.com/en-us/microsoft-365/extensibility/schema/root?view=m365-app-1.25&tabs=syntax

public class MsTeamsBotDto
{
    [Key]
    public int MsTeamsBotId { get; set; }

    [Required, MaxLength(256)]
    public string Version { get; set; } = "1.0.0";

    [Required]
    public Guid AppId { get; set; }

    [Required, MaxLength(30)]
    public string ShortName { get; set; } = null!;

    [MaxLength(100)]
    public string? FullName { get; set; } = "...";

    [Required, MaxLength(80)]
    public string ShortDescription { get; set; } = "...";

    [Required, MaxLength(4000)]
    public string? FullDescription { get; set; } = "...";

    [Required]
    public MsTeamsDeveloperDto Developer { get; set; } = null!;

    [Required, Url]
    public string ValidDomain { get; set; } = null!;

    public MsTeamsBotRegistrationDto Registration { get; set; } = null!;
}
