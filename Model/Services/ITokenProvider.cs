namespace Model.Services;

public interface ITokenProvider
{
    Task<string> GetAccessToken(string scope);
}