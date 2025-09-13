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
        switch (tags.Count)
        {
            case 0:
                return await SaveAndReturn();
            case < 1000:
            {
                var currentTagNames = tags.Select(t => t.Name).ToHashSet();
                var tagsFromApi = await GetThousandTags();
                var missingTags = tagsFromApi.Where(t => !currentTagNames.Contains(t.Name)).ToList();
            
                tags.AddRange(missingTags);
                return await SaveAndReturn(tags);
            }
            default:
                return tags;
        }
    }

    private static void CalculatePercent(List<Tag> tagList)
    {
        var totalCount = tagList.Sum(t => t.Count);
        foreach (var tag in tagList)
        {
            tag.Percentage = ((double)tag.Count / totalCount) * 100;
        }
    }
    private async Task<List<Tag>> SaveAndReturn(List<Tag>? tagList = null)
    {
        if (tagList == null)
        {
            var tags = await GetThousandTags();
            CalculatePercent(tags);
            await tagsDao.SaveTags(tags!);
            return tags;
        }
        
        CalculatePercent(tagList);
        await tagsDao.SaveTags(tagList!);
        return await tagsDao.GetTags();
    }

    private async Task<List<Tag>> GetThousandTags()
    {
        var client = httpClientFactory.CreateClient("StackOverflow");
        var tags = new List<Tag>();
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
        return tags;
    }
}