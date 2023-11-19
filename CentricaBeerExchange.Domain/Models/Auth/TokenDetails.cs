namespace CentricaBeerExchange.Domain.Models.Auth;

public class TokenDetails
{
    public TokenDetails(int userId, string tokenId, DateTime tokenExpiryUtc, string refreshToken, DateTime refreshExpiryUtc)
    {
        UserId = userId;
        TokenId = Guid.Parse(tokenId);
        TokenExpiryUtc = tokenExpiryUtc;
        RefreshToken = refreshToken;
        RefreshExpiryUtc = refreshExpiryUtc;
    }

    public int UserId { get; }
    public Guid TokenId { get; }
    public DateTime TokenExpiryUtc { get; }
    public string RefreshToken { get; }
    public DateTime RefreshExpiryUtc { get; }
}
