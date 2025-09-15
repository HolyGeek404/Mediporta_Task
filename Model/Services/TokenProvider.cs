using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace Model.Services;

public class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    public async Task<string> GetAccessToken(string scope)
    {
        var azure = configuration.GetSection("AzureAd");
        var tenantId = azure["TenantId"];
        var clientId = azure["ClientId"];
        var clientSecret = azure["ClientSecret"];
        var authority = $"https://login.microsoftonline.com/{tenantId}";

        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri(authority))
            .Build();

        try
        {
            var result =  app.AcquireTokenForClient([scope]).ExecuteAsync().Result;
            return result.AccessToken;
        
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return "";
    }
}