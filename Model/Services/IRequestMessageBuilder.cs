namespace Model.Services;

public interface IRequestMessageBuilder
{
    HttpRequestMessage BuildGet();
    RequestMessageBuilder AddBaseEndpoint(string baseEndpoint);
    RequestMessageBuilder AddPage(int pageNumber);
    RequestMessageBuilder AddPageSize(int pageSize);
    RequestMessageBuilder AddSort(string sort);
    RequestMessageBuilder AddOrder(string order);
    RequestMessageBuilder AddSite(string site);
}