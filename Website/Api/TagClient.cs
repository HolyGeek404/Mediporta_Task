using Model.DataAccess.Entities;
using Model.Features.Queries.GetTags;
using Model.Services;

namespace Website.Api;

public class TagClient(
    IConfiguration configuration,
    IHttpClientFactory clientFactory,
    IRequestMessageBuilder requestMessageBuilder,
    ILogger<TagClient> logger)
    : ITagClient
{
    public async Task<List<Tag>> GetTags(GetTagsQuery query)
    {
        logger.LogInformation("Fetching tags from Tag API: {@Query}", query);

        try
        {
            var client = clientFactory.CreateClient("TagClient");
            var scope = configuration.GetSection("TagApi")["Scope"];
            var url = UrlBuilderHelper.BuildTagApiUrl(query.Page, query.PageSize, query.Order, query.Sort);

            var request = await requestMessageBuilder.BuildGet(scope!, url);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Tag API returned non-success status code: {StatusCode}", response.StatusCode);
                return [];
            }

            var tagList = await response.Content.ReadFromJsonAsync<List<Tag>>();
            if (tagList == null || tagList.Count == 0)
            {
                logger.LogInformation("No tags returned from API for query: {@Query}", query);
                return [];
            }

            logger.LogInformation("Retrieved {Count} tags from API", tagList.Count);
            return tagList;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed when fetching tags for query: {@Query}", query);
            return [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred when fetching tags for query: {@Query}", query);
            return [];
        }
    }

    public async Task<List<Tag>> RefreshTags()
    {
        try
        {
            logger.LogInformation("Starting RefreshTags operation");

            var client = clientFactory.CreateClient("TagClient");
            var scope = configuration.GetSection("TagApi")["Scope"];
            var url = UrlBuilderHelper.BuildRefreshTagUrl();

            logger.LogInformation("Sending POST request to Tag API. URL: {Url}, Scope: {Scope}", url, scope);

            var request = await requestMessageBuilder.BuildPost(scope!, url);
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Tag API returned non-success status code: {StatusCode}", response.StatusCode);
                return [];
            }

            var tagList = await response.Content.ReadFromJsonAsync<List<Tag>>();

            if (tagList == null || tagList.Count == 0)
            {
                logger.LogInformation("Tag API returned no tags.");
                return [];
            }

            logger.LogInformation("Successfully retrieved {Count} tags from Tag API", tagList.Count);

            return tagList;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed when refreshing tags");
            return [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred when refreshing tags");
            return [];
        }
    }
}