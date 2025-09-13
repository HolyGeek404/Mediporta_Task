using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;
using Model.DataTransfer;

namespace Model.Services;

public class TagsService(
    ITagsDao tagsDao,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IRequestMessageBuilder requestMessageBuilder) : ITagsService
{
    public async Task<List<Tag>?> GetTags()
    {
        var tags = await tagsDao.GetTags();
        if (tags != null) return tags;
        
        var client = httpClientFactory.CreateClient("StackOverflow");
        var request = requestMessageBuilder.AddBaseEndpoint($"{configuration.GetSection("SO")["BaseAddress"]}/tags")
            .AddOrder("desc")
            .AddPage(1)
            .AddPageSize(5)
            .AddSort("popular")
            .AddSite("stackoverflow")
            .BuildGet();
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TagsResponse>();
        return result.Items;
    }
}