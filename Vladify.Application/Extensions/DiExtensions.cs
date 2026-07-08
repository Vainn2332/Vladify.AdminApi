using Microsoft.Extensions.DependencyInjection;
using Vladify.Application.Options;

namespace Vladify.Application.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            return services
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
    }
}
