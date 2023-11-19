namespace CentricaBeerExchange.Domain.Models.Auth;

public class AccessToken
{
    public AccessToken(string token, DateTime expiresAtUtc)
    {
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
    }

    public string Token { get; }
    public DateTime ExpiresAtUtc { get; }
}
