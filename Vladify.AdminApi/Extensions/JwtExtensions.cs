using System.Security.Claims;
using Vladify.Application.Constants;

namespace Vladify.AdminApi.Extensions;

public static class JwtExtensions
{
    extension(ClaimsPrincipal claims)
    {
        public string GetAuth0Id()
        {
            var auth0Id = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException(ErrorMessages.ClaimSubNotFound);

            return auth0Id;
        }
    }
}
