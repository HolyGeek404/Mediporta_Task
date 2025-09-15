namespace Model.Services;

public interface IRequestMessageBuilder
{
    HttpRequestMessage BuildGet(string endpoint);
    Task<HttpRequestMessage> BuildGet(string scope, string endpoint);
}