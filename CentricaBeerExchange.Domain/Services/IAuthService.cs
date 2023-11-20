using System.Security.Claims;

namespace CentricaBeerExchange.Domain.Services;

public interface IAuthService
{
    Task LoginAsync(string email);
    Task<TokenGenerationResult> GenerateTokenAsync(string email, int verificationCode);
    Task<TokenGenerationResult> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<bool> RevokeTokenAsync(ClaimsIdentity identity);
}
