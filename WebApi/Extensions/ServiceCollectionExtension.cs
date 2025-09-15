using System.Net.Http.Headers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Model.DataAccess;
using Model.DataAccess.Interfaces;
using Model.Services;
using Model.Services.Interfaces;

namespace WebApi.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<ITagsDao, TagsDao>();
        services.AddTransient<ITagsService, TagsService>();
        services.AddTransient<ITokenProvider, TokenProvider>();
        services.AddTransient<IRequestMessageBuilder, RequestMessageBuilder>();
        
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
    public static IServiceCollection AddMediatRConfig(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(TagsDao).Assembly));
        services.AddValidatorsFromAssemblyContaining<GetTagsQueryValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}