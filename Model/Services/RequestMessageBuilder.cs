using Microsoft.Extensions.Configuration;

namespace Model.Services;

public class RequestMessageBuilder(IConfiguration configuration) : IRequestMessageBuilder
{
    private HttpRequestMessage _request = new();
    private string? BaseEndpoint { get; set; }
    private string? Parameters { get; set; }

    public HttpRequestMessage BuildGet()
    {
        if (BaseEndpoint != null)
        {
            AddApiKey();
            _request.RequestUri = new Uri(BaseEndpoint+Parameters);
        }
        
        _request.Method = HttpMethod.Get;
        return _request;
    }

    private void AddApiKey()
    {
        var key = configuration.GetSection("SO")["ApiKey"];
        Parameters += $"key={key}";
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