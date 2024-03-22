using CentricaBeerExchange.Domain.Models.Auth;

namespace CentricaBeerExchange.DataAccess.MongoDb.Queries;

internal static class VerificationQueries
{
    const int VERIFICATION_CODE_VALID_MINUTES = 15;

    public static FilterDefinition<DbVerification> FilterByEmail(string email)
        => Builders<DbVerification>.Filter.Eq(v => v.Email, email);

    public static UpdateDefinition<DbVerification> Upsert(string email, int code, DateTime utcNow)
        => Builders<DbVerification>.Update
            .SetOnInsert(v => v.Id, Guid.NewGuid())
            .SetOnInsert(v => v.Email, email)
            .Set(v => v.CodeHash, CryptoHelper.GetHash(code))
            .Set(v => v.CreatedAtUtc, utcNow)
            .Set(v => v.ValidUntilUtc, utcNow.AddMinutes(VERIFICATION_CODE_VALID_MINUTES));
}

internal static class TokenQueries
{
    public static FilterDefinition<DbTokenDetails> FilterByUserId(int userId)
        => Builders<DbTokenDetails>.Filter.Eq(t => t.UserId, userId);

    public static UpdateDefinition<DbTokenDetails> Upsert(TokenDetails tokenDetails) 
        => Builders<DbTokenDetails>.Update
            .SetOnInsert(t => t.Id, Guid.NewGuid())
            .SetOnInsert(t => t.UserId, tokenDetails.UserId)
            .Set(t => t.TokenId, tokenDetails.TokenId)
            .Set(t => t.TokenExpiryUtc, tokenDetails.TokenExpiryUtc)
            .Set(t => t.RefreshToken, tokenDetails.RefreshToken)
            .Set(t => t.RefreshExpiryUtc, tokenDetails.RefreshExpiryUtc);
}
