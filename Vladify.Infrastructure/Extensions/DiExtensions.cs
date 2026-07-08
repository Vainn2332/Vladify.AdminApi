using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vladify.Application.Exceptions;

namespace Vladify.Infrastructure.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            return services
                .AddPostgresDb(configuration);
        }

        public IServiceCollection AddPostgresDb(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ApplicationDbContext")
                ?? throw new NotFoundException("Connection string for dbContext not found!");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            return services;
        }

    }
}
