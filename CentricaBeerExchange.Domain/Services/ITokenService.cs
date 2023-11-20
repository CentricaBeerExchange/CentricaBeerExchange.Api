namespace CentricaBeerExchange.Domain.Services;

public interface ITokenService
{
    TokenGenerationResult Generate(User user);
    bool TryGetIdFromExpiredToken(string accessToken, out Guid tokenId, [NotNullWhen(false)] out string? errorMessage);
}
