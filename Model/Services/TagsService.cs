using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;
using Model.DataTransfer;

namespace Model.Services;

public class TagsService(
    ITagsDao tagsDao,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : ITagsService
{
    public async Task<List<Tag>?> GetTags()
    {
        var tags = await tagsDao.GetTags();
        if (tags != null) return tags;
        tags = await UpdateTags();
        
        return tags;
    }

    private async Task<List<Tag>> UpdateTags()
    {
        var tags = new  List<Tag>();
        var client = httpClientFactory.CreateClient("StackOverflow");
        for (var i = 1; i <= 10; i++)
        {
            var request = new RequestMessageBuilder().AddBaseEndpoint($"{configuration.GetSection("SO")["BaseAddress"]}/tags")
                .AddOrder("desc")
                .AddPage(i)
                .AddPageSize(100)
                .AddSort("popular")
                .AddSite("stackoverflow")
                .BuildGet(configuration.GetSection("SO")["ApiKey"]!);
        
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<TagsResponse>();
            tags!.AddRange(result!.Items);
        }
        await tagsDao.SaveTags(tags!);
        return tags;
    } 
}