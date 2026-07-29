using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using Vladify.Application.Options;

namespace Vladify.AdminApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder endpointRouteBuilder)
    {
        public IEndpointRouteBuilder MapScalar(IConfiguration configuration)
        {
            endpointRouteBuilder.MapScalarApiReference(options =>
            {

                options.AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme)
                    .AddAuthorizationCodeFlow(JwtBearerDefaults.AuthenticationScheme, flow =>
                    {
                        var auth0Options = configuration.GetSection(Auth0Options.SectionName).Get<Auth0Options>()
                            ?? throw new ArgumentException($"Configuration section{Auth0Options.SectionName} not found!");

                        flow.ClientId = auth0Options.ClientId;
                        flow.Pkce = Pkce.Sha256;
                        flow.AddQueryParameter("audience", auth0Options.Audience);
                    });
            });

            return endpointRouteBuilder;
        }
    }
}
