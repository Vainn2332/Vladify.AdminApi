using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vladify.Application.Options;

namespace Vladify.AdminApi.Extensions;

public static class ApiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApiServices()
        {
            return services
                .AddJwtBasedAuthentication()
                .AddPolicyBasedAuthorization();
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

        public IServiceCollection AddPolicyBasedAuthorization()
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOrModerator", policy =>
                {
                    policy.RequireClaim("https://my-app.com/roles", "Admin", "Moderator");
                });
            });

            return services;
        }
    }
}
