using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Vladify.Infrastructure.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(string connectionString)
        {
            return services
                .AddPostgresDb(connectionString);
        }

        public IServiceCollection AddPostgresDb(string connectionString)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}
