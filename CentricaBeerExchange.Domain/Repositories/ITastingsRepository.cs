

namespace CentricaBeerExchange.Domain.Repositories;

public interface ITastingsRepository
{
    Task<Tasting[]> GetAsync();
    Task<Tasting?> GetAsync(int id);
    Task<Tasting> AddAsync(DateOnly date, string? theme);
    Task<Tasting> UpdateAsync(int id, DateOnly date, string? theme);
    Task DeleteAsync(int id);
}
