using System.Net.Http.Headers;

namespace Model.Services;

public class RequestMessageBuilder(ITokenProvider tokenProvider) : IRequestMessageBuilder
{
    public HttpRequestMessage BuildGet(string endpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        return request;
    }
    public async Task<HttpRequestMessage> BuildGet(string scope, string endpoint)
    {
        var token = await tokenProvider.GetAccessToken(scope);
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}