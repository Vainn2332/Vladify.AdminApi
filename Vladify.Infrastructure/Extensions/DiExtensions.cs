using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Polly;
using Polly.Retry;
using Vladify.Infrastructure.Constants;

namespace Vladify.Infrastructure.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(string connectionString)
        {
            return services
                .AddPolly()
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

        public IServiceCollection AddPolly()
        {
            services.AddResiliencePipeline(PollyPipelineConstants.DbRetryPipelineName, builder =>
            {
                builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = PollyPipelineConstants.MaxAmountOfRetries,
                    Delay = PollyPipelineConstants.Delay,
                    BackoffType = DelayBackoffType.Exponential,
                    ShouldHandle = new PredicateBuilder().Handle<NpgsqlException>(ex => ex.IsTransient)
                });
            });

            return services;
        }
    }
}
