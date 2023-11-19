namespace CentricaBeerExchange.Domain.Repositories;

public interface IAuthRepository
{
    Task UpsertVerificationCodeAsync(string email, int code);
    Task<Verification?> GetVerificationAsync(string email);
    Task RemoveVerificationAsync(string email);

    Task<TokenDetails?> GetTokenAsync(Guid tokenId);
    Task UpsertTokenAsync(TokenDetails tokenDetails);
    Task<bool> RemoveTokenAsync(int userId);

    Task<User> GetUserAsync(int id);
    Task<User> GetOrCreateUserAsync(string email);
}
