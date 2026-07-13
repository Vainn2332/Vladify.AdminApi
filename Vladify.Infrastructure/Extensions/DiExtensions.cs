using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Polly;
using Polly.Retry;
using Vladify.Application.Interfaces;
using Vladify.Infrastructure.Constants;
using Vladify.Infrastructure.Repositories;

namespace Vladify.Infrastructure.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(string connectionString)
        {
            return services
                .AddPolly()
                .AddRepositories()
                .AddPostgresDb(connectionString);
        }

        public IServiceCollection AddRepositories()
        {
            services.AddScoped<IModerationTaskRepository, ModerationTaskRepository>();

            return services;
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
