using System.ComponentModel.DataAnnotations;

namespace AiLabs.Repository.Entities;

public class MsTeamsDeveloperDto
{
    [Key]
    public int MsTeamsDeveloperId { get; set; }

    [Required, MaxLength(32)]
    public string Name { get; set; } = null!;

    [Required, Url]
    public string WebsiteUrl { get; set; } = null!;

    [Required, Url]
    public string PrivacyUrl { get; set; } = null!;

    [Required, Url]
    public string TermsOfUseUrl { get; set; } = null!;

}
