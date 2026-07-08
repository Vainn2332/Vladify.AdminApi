using System.ComponentModel.DataAnnotations;

namespace Vladify.Application.Options;

public class Auth0Options
{
    public const string SectionName = "Auth0Options";

    [Required]
    public required string Domain { get; set; }

    [Required]
    public required string Audience { get; set; }

    public string Authority => $"https://{Domain}";
}
