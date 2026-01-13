using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using OptimizelySaaSStyleManager.Services;

namespace OptimizelySaaSStyleManager.Configuration;

public static class ServiceCollectionExtensions
{
    private const string UserAgent = "OptimizelySaaSStyleManager/1.0";

    public static IServiceCollection AddOptimizelyStyleManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OptimizelyApiSettings>(
            configuration.GetSection(OptimizelyApiSettings.SectionName));

        // Register the token service with proper headers
        services.AddHttpClient<ITokenService, TokenService>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

        // Register the bearer token handler
        services.AddTransient<BearerTokenHandler>();

        // Register the style service with bearer token authentication and proper headers
        services.AddHttpClient<IStyleService, StyleService>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<OptimizelyApiSettings>>().Value;

            // Ensure base URL ends with / for proper path resolution
            var baseUrl = settings.BaseUrl;
            if (!baseUrl.EndsWith('/'))
            {
                baseUrl += "/";
            }

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<BearerTokenHandler>();

        services.AddScoped<IStyleExportService, StyleExportService>();

        return services;
    }
}
