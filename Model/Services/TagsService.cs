using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;
using Model.DataTransfer;
using Model.Features.Queries.GetTags;
using Model.Services.Interfaces;

namespace Model.Services;

public class TagsService(ITagsDao tagsDao, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    : ITagsService
{
    public async Task<List<Tag>> GetTags(GetTagsQuery query, CancellationToken cancellationToken)
    {
        var tags = await tagsDao.GetTags(query);
        if (tags.Count == 0)
        {
            return await SaveAndReturn(query);
        }

        return tags;
    }

    public async Task UpdateTags()
    {
        var allTags = await tagsDao.GetAllTags();
        if (allTags.Count < 1000)
        {
            var currentTagNames = allTags.Select(t => t.Name).ToHashSet();
            var tagsFromApi = await GetThousandTags(new GetTagsQuery());
            var missingTags = tagsFromApi.Where(t => !currentTagNames.Contains(t.Name)).ToList();
            allTags.AddRange(missingTags);
            CalculatePercent(allTags);
            await tagsDao.SaveTags(missingTags);
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

    private async Task<List<Tag>> SaveAndReturn(GetTagsQuery query, List<Tag>? tagList = null)
    {
        if (tagList == null)
        {
            var tags = await GetThousandTags(query);
            CalculatePercent(tags);
            await tagsDao.SaveTags(tags!);
            return tags;
        }

        CalculatePercent(tagList);
        await tagsDao.SaveTags(tagList!);
        return await tagsDao.GetTags(query);
    }

    private async Task<List<Tag>> GetThousandTags(GetTagsQuery query)
    {
        var client = httpClientFactory.CreateClient("StackOverflow");
        var tags = new List<Tag>();
        for (var i = 1; i <= 10; i++)
        {
            query.Page = i;
            var requestMessage = new RequestMessageBuilder(configuration).BuildGet(query, configuration.GetSection("SO")["ApiKey"]!);
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TagsResponse>();
            tags!.AddRange(result!.Items);
        }

        return tags;
    }
}