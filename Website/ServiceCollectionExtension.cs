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
            client.BaseAddress = new Uri(configuration.GetSection("TagApi")["BaseAddress"]!);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}