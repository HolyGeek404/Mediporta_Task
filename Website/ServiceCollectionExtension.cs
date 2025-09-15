using System.Net.Http.Headers;
using Model.Services;
using Website.Api;

namespace Website;

public static class ServiceCollectionExtension
{

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<ITagClient, TagClient>();
        services.AddTransient<IRequestMessageBuilder, RequestMessageBuilder>();
        services.AddTransient<ITokenProvider, TokenProvider>();
        
        return services;
    }
    public static IServiceCollection AddTagsApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("TagClient",client =>
        {
            var isDocker = Environment.GetEnvironmentVariable("IsDocker")!;
            string apiUrl;
            if (!string.IsNullOrEmpty(isDocker) && isDocker.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                apiUrl = configuration.GetSection("TagApi")["DockerAddress"]!;
            }
            else
            {
                apiUrl = configuration.GetSection("TagApi")["BaseAddress"]!;
            }

            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}