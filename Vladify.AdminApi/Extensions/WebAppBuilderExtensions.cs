using Microsoft.IdentityModel.Protocols.Configuration;
using Vladify.Infrastructure.Extensions;

namespace Vladify.AdminApi.Extensions;

public static class WebAppBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder ConfigureInfrastructure()
        {
            var connectionString = builder.Configuration.GetConnectionString("AdminApiDb")
                ?? throw new InvalidConfigurationException("Failed to get connection string for dbContext!");

            builder.Services.AddInfrastructure(connectionString);

            return builder;
        }
    }
}
