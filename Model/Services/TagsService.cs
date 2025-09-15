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
    IConfiguration configuration,
    IRequestMessageBuilder requestMessageBuilder)
    : ITagsService
{
    public async Task<List<Tag>> GetTags(GetTagsQuery query, CancellationToken cancellationToken)
    {
        var tags = await tagsDao.GetTags(query);
        if (tags.Count == 0)
        {
            return await GetTagsFromSoApi(query.Page, query.Order, query.PageSize, query.Sort);
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
            var tagsFromApi = new List<Tag>();
            for (var i = 1; i <= 10; i++)
            {
                tagsFromApi.AddRange(await GetTagsFromSoApi(i, command.Order, command.PageSize,command.Sort));
            }
            var missingTags = tagsFromApi.Where(t => !currentTagNames.Contains(t.Name)).ToList();
            allTags.AddRange(missingTags);
            CalculatePercent(allTags);
            await tagsDao.SaveTags(missingTags);
        }
    }

    public async Task<List<Tag>> RefreshTags(RefreshTagsCommand  refreshTagsCommand)
    {
        await tagsDao.DeleteAllTags();
        var tags = new List<Tag>();
        for (var i = 1; i <= 10; i++)
        {
            tags.AddRange(await GetTagsFromSoApi(refreshTagsCommand.Page , refreshTagsCommand.Order, refreshTagsCommand.PageSize, refreshTagsCommand.Sort));
        }
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
    
    private async Task<List<Tag>> GetTagsFromSoApi(int page,string order, int pageSize, string sort)
    {
        var client = httpClientFactory.CreateClient("StackOverflow");
        var tags = new List<Tag>();
        
        var apiKey = configuration.GetSection("SO")["ApiKey"];
        var url = UrlBuilderHelper.BuildStackOverflowTagUrl(page,pageSize,order,sort,apiKey!);
        var requestMessage = requestMessageBuilder.BuildGet(url);
        
        var response = await client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<TagsResponse>();
        tags!.AddRange(result!.Items);
        

        return tags;
    }
}