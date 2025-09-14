namespace Model.Services;

public class RequestMessageBuilder()
{
    private string BaseEndpoint { get; set; }
    private string Parameters { get; set; }

    public HttpRequestMessage BuildGet(string baseAddress, string apiKey)
    {
        var request = new HttpRequestMessage();

        AddBaseEndpoint("tags");
        AddSite("stackoverflow");
        AddApiKey(apiKey);
        
        request.RequestUri = new Uri(baseAddress + BaseEndpoint + Parameters);
        request.Method = HttpMethod.Get;
        return request;
    }

    private void AddApiKey(string apiKey)
    {
        Parameters += $"key={apiKey}";
    }

    private void AddBaseEndpoint(string baseEndpoint)
    {
        BaseEndpoint = $"{baseEndpoint}?";
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

    private void AddSite(string site)
    {
        Parameters += $"site={site}&";
    }
}