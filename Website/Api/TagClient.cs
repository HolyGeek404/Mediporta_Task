using Model.DataAccess.Entities;
using Model.Features.Queries.GetTags;
using Model.Services;

namespace Website.Api;

public class TagClient(
    IConfiguration configuration,
    IHttpClientFactory clientFactory,
    IRequestMessageBuilder requestMessageBuilder) : ITagClient
{
    public async Task<List<Tag>> GetTags(GetTagsQuery query)
    {
        var client = clientFactory.CreateClient("TagClient");
        var scope = configuration.GetSection("TagApi")["Scope"];
        var url = UrlBuilderHelper.BuildTagApiUrl(query.Page, query.PageSize,query.Order,query.Sort);
        var request = await requestMessageBuilder.BuildGet(scope!, url);

        var response = await client.SendAsync(request);
        var tagList = await response.Content.ReadFromJsonAsync<List<Tag>>();

        return tagList ?? [];
    }
}