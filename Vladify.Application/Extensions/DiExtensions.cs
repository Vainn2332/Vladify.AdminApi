using Microsoft.Extensions.DependencyInjection;
using Vladify.Application.MapperProfiles;
using Vladify.Application.Options;

namespace Vladify.Application.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            return services
                .AddAutoMapper()
                .ConfigureOptions();
        }

        public IServiceCollection ConfigureOptions()
        {
            services
                .AddOptions<Auth0Options>()
                .BindConfiguration(Auth0Options.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        public IServiceCollection AddAutoMapper()
        {
            services.AddAutoMapper(cfg => { }, typeof(ModerationTaskProfile).Assembly);

            return services;
        }
    }
}
