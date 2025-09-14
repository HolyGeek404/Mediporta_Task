using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;
using Model.DataTransfer;
using Model.Features.Commands.RefreshTags;
using Model.Features.Queries.GetTags;
using Model.Services.Interfaces;

namespace Model.Services;

public class TagsService(
    ITagsDao tagsDao,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration)
    : ITagsService
{
    public async Task<List<Tag>> GetTags(GetTagsQuery query, CancellationToken cancellationToken)
    {
        var tags = await tagsDao.GetTags(query);
        if (tags.Count == 0)
        {
            return await GetThousandTags(query.Order, query.PageSize, query.Sort);
        }

        return tags;
    }

    public async Task UpdateTags()
    {
        var allTags = await tagsDao.GetAllTags();
        if (allTags.Count < 1000)
        {
            var currentTagNames = allTags.Select(t => t.Name).ToHashSet();
            var command = new RefreshTagsCommand();
            var tagsFromApi = await GetThousandTags(command.Order, command.PageSize,command.Sort);
            var missingTags = tagsFromApi.Where(t => !currentTagNames.Contains(t.Name)).ToList();
            allTags.AddRange(missingTags);
            CalculatePercent(allTags);
            await tagsDao.SaveTags(missingTags);
        }
    }

    public async Task<List<Tag>> RefreshTags(RefreshTagsCommand  refreshTagsCommand)
    {
        var tags = await GetThousandTags(refreshTagsCommand.Order, refreshTagsCommand.PageSize, refreshTagsCommand.Sort);
        CalculatePercent(tags);
        await tagsDao.SaveTags(tags);
        return tags;
    }

    private static void CalculatePercent(List<Tag> tagList)
    {
        var totalCount = tagList.Sum(t => t.Count);
        foreach (var tag in tagList)
        {
            tag.Percentage = ((double)tag.Count / totalCount) * 100;
        }
    }
    
    private async Task<List<Tag>> GetThousandTags(string order, int pageSize, string sort)
    {
        var client = httpClientFactory.CreateClient("StackOverflow");
        var tags = new List<Tag>();
        for (var i = 1; i <= 10; i++)
        {
            var requestMessage = new RequestMessageBuilder(configuration)
                .AddBaseEndpoint(GetTagsQuery.BaseEndpoint)
                .AddOrder(order)
                .AddPage(i)
                .AddPageSize(pageSize)
                .AddSort(sort)
                .BuildGet(configuration.GetSection("SO")["BaseAddress"]!,
                    configuration.GetSection("SO")["ApiKey"]!);
            
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TagsResponse>();
            tags!.AddRange(result!.Items);
        }

        return tags;
    }
}