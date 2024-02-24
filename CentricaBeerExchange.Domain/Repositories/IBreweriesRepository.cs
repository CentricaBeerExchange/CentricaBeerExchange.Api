namespace CentricaBeerExchange.Domain.Repositories;

public interface IBreweriesRepository
{
    Task<Brewery[]> GetAsync();
    Task<Brewery?> GetAsync(int id);

    Task<Brewery> AddAsync(string name, string? untappdId, string? location, string? type, string? thumbnail);
    Task<Brewery> UpdateAsync(int id, string name, string? untappdId, string? location, string? type, string? thumbnail);

    Task DeleteAsync(int id);
}
