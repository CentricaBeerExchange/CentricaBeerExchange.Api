namespace CentricaBeerExchange.Domain.Repositories;

public interface IProfileRepository
{
    Task<Profile[]> GetAsync();
    Task<Profile?> GetAsync(int id);
    Task<Profile?> UpdateAsync(int id, Profile updatedProfile);
}
