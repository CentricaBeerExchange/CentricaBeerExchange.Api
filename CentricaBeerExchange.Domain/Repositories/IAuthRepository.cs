namespace CentricaBeerExchange.Domain.Repositories;

public interface IAuthRepository
{
    Task UpsertVerificationCodeAsync(string email, int code);
    Task<Verification?> GetVerificationAsync(string email);
    Task RemoveVerificationAsync(string email);
    Task<User> GetUserAsync(int id);
    Task<User> GetOrCreateUserAsync(string email);
    Task<User> UpdateUserEmailAsync(int id, string newEmail);
}
