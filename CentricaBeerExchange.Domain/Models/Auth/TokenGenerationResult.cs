namespace CentricaBeerExchange.Domain.Models.Auth;

public class TokenGenerationResult
{
    public TokenGenerationResult(string errorMessage, bool isUnauthorized)
    {
        Successful = false;

        ErrorMessage = errorMessage;
        IsUnauthorized = isUnauthorized;

        AccessToken = string.Empty;
    }

    public TokenGenerationResult(string accessToken, DateTime expiresAtUtc)
    {
        Successful = true;

        AccessToken = accessToken;
        ExpiresAtUtc = expiresAtUtc;

        ErrorMessage = string.Empty;
    }

    public bool Successful { get; }
    public string AccessToken { get; }
    public DateTime ExpiresAtUtc { get; }

    public string ErrorMessage { get; }
    public bool IsUnauthorized { get; }
}
