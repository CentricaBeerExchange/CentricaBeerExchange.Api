namespace CentricaBeerExchange.DataAccess.MongoDb.Models;

internal class DbTokenDetails(Guid id, int userId, Guid tokenId, DateTime tokenExpiryUtc, string refreshToken, DateTime refreshExpiryUtc)
{
    public Guid Id { get; } = id;
    public int UserId { get; } = userId;
    public Guid TokenId { get; } = tokenId;
    public DateTime TokenExpiryUtc { get; } = tokenExpiryUtc;
    public string RefreshToken { get; } = refreshToken;
    public DateTime RefreshExpiryUtc { get; } = refreshExpiryUtc;

    public TokenDetails Map()
        => new(UserId, TokenId.ToString(), TokenExpiryUtc, RefreshToken, RefreshExpiryUtc);
}
