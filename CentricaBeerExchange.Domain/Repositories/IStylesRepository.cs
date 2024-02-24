namespace CentricaBeerExchange.Domain.Repositories;

public interface IStylesRepository
{
    Task<Style[]> GetAsync();
    Task<Style?> GetAsync(short id);

    Task<Style> UpsertAsync(Style style);
    Task<Style[]> UpsertAsync(Style[] styles);

    Task DeleteAsync(short id);
    Task DeleteAsync(short[] ids);
}
