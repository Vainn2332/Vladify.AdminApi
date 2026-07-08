using System.ComponentModel.DataAnnotations;

namespace Vladify.Application.Options;

public class Auth0Options
{
    public const string SectionName = "Auth0Options";

    [Required]
    public required string Authority { get; set; }

    [Required]
    public required string Audience { get; set; }
}
