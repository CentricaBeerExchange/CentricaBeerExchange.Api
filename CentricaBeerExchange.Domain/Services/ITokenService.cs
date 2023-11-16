namespace CentricaBeerExchange.Domain.Services;

public interface ITokenService
{
    TokenGenerationResult Generate(User user);
}
