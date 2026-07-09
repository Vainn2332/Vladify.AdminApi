using Vladify.Infrastructure.Extensions;

namespace Vladify.AdminApi.Extensions;

public static class WebAppBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder ConfigureInfrastructure()
        {
            var connectionString = builder.Configuration.GetConnectionString("AdminApiDb")
                ?? throw new InvalidOperationException("Connection string for dbContext not found!");

            builder.Services.AddInfrastructure(connectionString);

            return builder;
        }
    }
}
