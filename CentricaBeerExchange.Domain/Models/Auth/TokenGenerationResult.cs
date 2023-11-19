namespace CentricaBeerExchange.Domain.Models.Auth;

public class TokenGenerationResult
{
    public TokenGenerationResult(string errorMessage, bool isUnauthorized)
    {
        Successful = false;

        Email = string.Empty;
        ErrorMessage = errorMessage;
        IsUnauthorized = isUnauthorized;
    }

    public TokenGenerationResult(string email, Guid tokenId, AccessToken accessToken, AccessToken refreshToken)
    {
        Successful = true;

        Email = email;
        TokenId = tokenId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;

        ErrorMessage = string.Empty;
    }

    public bool Successful { get; }
    public string Email { get; }
    public Guid TokenId { get; }
    public AccessToken? AccessToken { get; }
    public AccessToken? RefreshToken { get; }

    public string ErrorMessage { get; }
    public bool IsUnauthorized { get; }

    public TokenDetails AsTokenDetails(int userId)
        => new(userId, TokenId.ToString(), AccessToken!.ExpiresAtUtc, RefreshToken!.Token, RefreshToken.ExpiresAtUtc);
}
