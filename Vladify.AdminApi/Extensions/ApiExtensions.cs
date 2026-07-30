using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Vladify.AdminApi.Constants;
using Vladify.AdminApi.Grpc.ModerationTask;
using Vladify.Application.Constants;
using Vladify.Application.Exceptions;
using Vladify.Application.Options;

namespace Vladify.AdminApi.Extensions;

public static class ApiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApiServices(IConfiguration configuration)
        {
            services
                .AddJwtBasedAuthentication()
                .AddPolicyBasedAuthorization()
                .AddOpenApiDocumentation(configuration);

            services.AddGrpc();

            return services;
        }

        public IServiceCollection AddJwtBasedAuthentication()
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<Auth0Options>>((options, auth0) =>
            {
                var auth0Options = auth0.Value;

                options.Authority = auth0Options.Authority;
                options.Audience = auth0Options.Audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            return services;
        }

        public IServiceCollection AddOpenApiDocumentation(IConfiguration configuration)
        {
            var auth0Options = configuration.GetSection(Auth0Options.SectionName).Get<Auth0Options>()
                ?? throw new NotFoundException($"Configuration section{Auth0Options.SectionName} not found!");

            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(auth0Options.AuthorizationUrl),
                                TokenUrl = new Uri(auth0Options.TokenUrl),
                                Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID" },
                                { "profile", "Profile" },
                                { "email", "Email" }
                            }
                            }
                        }
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                    return Task.CompletedTask;
                });
            });

            return services;
        }

        public IServiceCollection AddPolicyBasedAuthorization()
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.AdminOrModerator, policy =>
                {
                    policy.RequireClaim(JwtClaims.Roles, AppRoles.Admin, AppRoles.Moderator);
                });
            });

            return services;
        }
    }

    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder ConfigureGrpcServices()
        {
            app.MapGrpcService<ModerationGrpcService>();

            return app;
        }
    }
}
