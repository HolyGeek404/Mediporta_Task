using System.Net.Http.Headers;
using Model.DataAccess;
using Model.DataAccess.Interfaces;
using Model.Services;

namespace WebApi.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<ITagsDao, TagsDao>();
        services.AddTransient<ITagsService, TagsService>();
        
        
        return services;
    }
    public static IServiceCollection AddStackOverflowApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("StackOverflow",client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection("SO")["BaseAddress"]!);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mediporta_Task");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}