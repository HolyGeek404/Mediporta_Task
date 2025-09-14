using Model.Features.Queries.GetTags;

namespace Model.Services;

public class RequestMessageBuilder
{
    private string BaseEndpoint { get; set; }
    private string Parameters { get; set; }

    public HttpRequestMessage BuildGet(GetTagsQuery query, string apiKey)
    {
        var request = new HttpRequestMessage();

        AddBaseEndpoint(GetTagsQuery.BaseEndpoint);
        AddOrder(query.Order);
        AddPage(query.Page);
        AddPageSize(query.PageSize);
        AddSort(query.Sort);
        AddSort(GetTagsQuery.Site);
        AddApiKey(apiKey);
        
        request.RequestUri = new Uri(BaseEndpoint + Parameters);
        request.Method = HttpMethod.Get;
        return request;
    }

    private void AddApiKey(string apiKey)
    {
        Parameters += $"key={apiKey}";
    }

    private RequestMessageBuilder AddBaseEndpoint(string baseEndpoint)
    {
        BaseEndpoint = $"{baseEndpoint}?";
        return this;
    }

    private void AddPage(int pageNumber)
    {
        Parameters += $"page={pageNumber}&";
    }

    private void AddPageSize(int pageSize)
    {
        Parameters += $"pagesize={pageSize}&";
    }

    private void AddSort(string sort)
    {
        Parameters += $"sort={sort}&";
    }

    private void AddOrder(string order)
    {
        Parameters += $"order={order}&";
    }

    private void AddSite(string site)
    {
        Parameters += $"site={site}&";
    }
}