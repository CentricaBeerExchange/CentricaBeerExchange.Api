namespace CentricaBeerExchange.DataAccess;

public class TastingsRepository : ITastingsRepository
{
    private readonly IDbConnection _connection;

    public TastingsRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Tasting[]> GetAsync()
    {
        string sql = "SELECT Id, Date, Theme " +
                     "FROM beer_exchange.Tastings";

        IEnumerable<Tasting> query = await _connection.QueryAsync<Tasting>(sql);

        return query?.ToArray() ?? [];
    }

    public async Task<Tasting?> GetAsync(int id)
    {
        string sql = "SELECT Id, Date, Theme " +
                     "FROM beer_exchange.Tastings " +
                     "WHERE Id = @id";

        Tasting? tasting = await _connection.QuerySingleOrDefaultAsync<Tasting>(
            sql: sql,
            param: new { id }
        );

        return tasting;
    }

    public async Task<Tasting> AddAsync(DateOnly date, string? theme)
    {
        string sql = "INSERT INTO beer_exchange.Tastings (Date, Theme) " +
                     "VALUES (@date, @theme);" +
                     "SELECT LAST_INSERT_ID();";

        int insertedId = await _connection.QuerySingleOrDefaultAsync<int>(
            sql: sql,
            param: new { date, theme }
        );

        Tasting? tasting = await GetAsync(insertedId);
        return tasting ?? throw new InvalidOperationException("Tasting NOT found after insert!");
    }

    public async Task<Tasting> UpdateAsync(int id, DateOnly date, string? theme)
    {
        string sql = "UPDATE beer_exchange.Tastings " +
                     "SET Date = @date, Theme = @theme " +
                     "WHERE Id = @id";

        await _connection.QuerySingleOrDefaultAsync<int>(
            sql: sql,
            param: new { id, date, theme }
        );

        Tasting? tasting = await GetAsync(id);
        return tasting ?? throw new InvalidOperationException("Tasting NOT found after update!");
    }

    public Task DeleteAsync(int id)
    {
        string sql = "DELETE FROM beer_exchange.Tastings " +
                     "WHERE Id = @id";

        return _connection.ExecuteAsync(
            sql: sql,
            param: new { id }
        );
    }
}
