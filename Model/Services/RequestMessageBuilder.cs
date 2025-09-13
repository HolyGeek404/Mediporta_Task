using Microsoft.Extensions.Configuration;

namespace Model.Services;

public class RequestMessageBuilder
{
    private string? BaseEndpoint { get; set; }
    private string? Parameters { get; set; }

    public HttpRequestMessage BuildGet(string apiKey)
    {
        var request = new HttpRequestMessage();
        if (BaseEndpoint != null)
        {
            AddApiKey(apiKey);
            request.RequestUri = new Uri(BaseEndpoint+Parameters);
        }
        
        request.Method = HttpMethod.Get;
        return request;
    }

    private void AddApiKey(string apiKey)
    {
        Parameters += $"key={apiKey}";
    }
    
    public RequestMessageBuilder AddBaseEndpoint(string baseEndpoint)
    {
        BaseEndpoint ??= $"{baseEndpoint}?";
        return this;
    }
    
    public RequestMessageBuilder AddPage(int pageNumber)
    {
        Parameters += $"page={pageNumber}&";
        return this;
    }

    public RequestMessageBuilder AddPageSize(int pageSize)
    {
        Parameters += $"pagesize={pageSize}&";
        return this;
    }
    
    public RequestMessageBuilder AddSort(string sort)
    {
        Parameters += $"sort={sort}&";
        return this;
    }

    public RequestMessageBuilder AddOrder(string order)
    {
        Parameters += $"order={order}&";
        return this;
    }

    public RequestMessageBuilder AddSite(string site)
    {
        Parameters += $"site={site}&";
        return this;
    }
}