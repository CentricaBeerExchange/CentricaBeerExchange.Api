using CentricaBeerExchange.Domain.Models.Auth;

namespace CentricaBeerExchange.Domain.Services;

public interface ITokenService
{
    Task<TokenGenerationResult> GetAsync(string clientId, string clientSecret);
}
