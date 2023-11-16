namespace CentricaBeerExchange.Domain.Services;

public interface IAuthService
{
    Task LoginAsync(string email);
    Task<TokenGenerationResult> GetTokenAsync(string email, int verificationCode);
}
