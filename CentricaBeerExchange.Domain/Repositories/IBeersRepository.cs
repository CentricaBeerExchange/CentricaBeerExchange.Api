
namespace CentricaBeerExchange.Domain.Repositories;

public interface IBeersRepository
{
    Task<Beer[]> GetAsync();
    Task<Beer[]> GetAsync(int[] ids);
    Task<Beer?> GetAsync(int id);
    Task<Beer> AddAsync(string name, int breweryId, short styleId, decimal? rating, decimal? abv, int? untappdId);
    Task<Beer> UpdateAsync(int id, string name, int breweryId, short styleId, decimal? rating, decimal? abv, int? untappdId);
    Task DeleteAsync(int id);
}
