using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    IRequestMessageBuilder requestMessageBuilder,
    ILogger<TagsService> logger) : ITagsService
{
    public async Task<List<Tag>> GetTags(GetTagsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Fetching tags from DB with query {@Query}", query);

            var tags = await tagsDao.GetTags(query);
            if (tags.Count == 0)
            {
                logger.LogWarning("No tags found in DB. Fetching from external API...");
                return await GetTagsFromSoApi(query.Page, query.Order, query.PageSize, query.Sort);
            }

            logger.LogInformation("Retrieved {Count} tags from DB", tags.Count);
            return tags;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting tags with query {@Query}", query);
            throw; 
        }
    }

    public async Task UpdateTags()
    {
        try
        {
            logger.LogInformation("Updating tags...");

            var allTags = await tagsDao.GetAllTags();
            if (allTags.Count < 1000)
            {
                logger.LogInformation("Current tags count {Count} < 1000. Fetching additional tags...", allTags.Count);

                var currentTagNames = allTags.Select(t => t.Name).ToHashSet();
                var command = new RefreshTagsCommand();
                var tagsFromApi = new List<Tag>();

                for (var i = 1; i <= 10; i++)
                {
                    logger.LogDebug("Fetching tags from API, page {Page}", i);
                    tagsFromApi.AddRange(await GetTagsFromSoApi(i, command.Order, command.PageSize, command.Sort));
                }

                var missingTags = tagsFromApi.Where(t => !currentTagNames.Contains(t.Name)).ToList();
                logger.LogInformation("Found {Count} missing tags", missingTags.Count);

                allTags.AddRange(missingTags);
                CalculatePercent(allTags);

                await tagsDao.SaveTags(missingTags);
                logger.LogInformation("Saved {Count} new tags", missingTags.Count);
            }
            else
            {
                logger.LogInformation("Tags already up-to-date. Count: {Count}", allTags.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while updating tags");
            throw;
        }
    }

    public async Task<List<Tag>> RefreshTags(RefreshTagsCommand refreshTagsCommand)
    {
        try
        {
            logger.LogInformation("Refreshing tags with command {@Command}", refreshTagsCommand);

            await tagsDao.DeleteAllTags();
            var tags = new List<Tag>();

            for (var i = 1; i <= 10; i++)
            {
                logger.LogDebug("Fetching page {Page} from API", i);
                tags.AddRange(await GetTagsFromSoApi(
                    refreshTagsCommand.Page,
                    refreshTagsCommand.Order,
                    refreshTagsCommand.PageSize,
                    refreshTagsCommand.Sort));
            }

            CalculatePercent(tags);
            await tagsDao.SaveTags(tags);

            logger.LogInformation("Refreshed {Count} tags", tags.Count);
            return tags;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while refreshing tags with command {@Command}", refreshTagsCommand);
            throw;
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

    private async Task<List<Tag>> GetTagsFromSoApi(int page, string order, int pageSize, string sort)
    {
        try
        {
            var client = httpClientFactory.CreateClient("StackOverflow");

            var apiKey = configuration.GetSection("SO")["ApiKey"];
            var url = UrlBuilderHelper.BuildStackOverflowTagUrl(page, pageSize, order, sort, apiKey!);

            logger.LogDebug("Requesting tags from external API. URL={Url}", url);

            var requestMessage = requestMessageBuilder.BuildGet(url);
            var response = await client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TagsResponse>();

            if (result?.Items == null)
            {
                logger.LogWarning("API returned no items on page {Page}", page);
                return [];
            }

            logger.LogInformation("Received {Count} tags from API (page {Page})", result.Items.Count, page);
            return result.Items;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while fetching tags from external API (page={Page}, pageSize={PageSize}, order={Order}, sort={Sort})",
                page, pageSize, order, sort);
            throw;
        }
    }
}