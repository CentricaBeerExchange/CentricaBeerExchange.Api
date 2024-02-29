namespace CentricaBeerExchange.DataAccess;

public class StylesRepository : IStylesRepository
{
    private readonly IDbConnection _connection;

    public StylesRepository(IDbConnection connection)
    {
        _connection = connection;
        _connection.Open();
    }

    public async Task<Style[]> GetAsync()
    {
        string sql = "SELECT Id, Name " +
                     "FROM beer_exchange.Styles " +
                     "WHERE IsActive = 1";

        IEnumerable<Style> query = await _connection.QueryAsync<Style>(sql);

        return query?.ToArray() ?? [];
    }

    public async Task<Style?> GetAsync(short id)
    {
        string sql = "SELECT Id, Name " +
                     "FROM beer_exchange.Styles " +
                     "WHERE Id = @id";

        Style? style = await _connection.QuerySingleOrDefaultAsync<Style>(
            sql: sql,
            param: new { id }
        );

        return style;
    }

    public async Task<Style[]> GetAsync(short[] ids)
    {
        string sql = "SELECT Id, Name " +
                     "FROM beer_exchange.Styles " +
                     "WHERE Id IN @ids";

        IEnumerable<Style> query = await _connection.QueryAsync<Style>(
            sql: sql,
            param: new { ids }
        );

        return query?.ToArray() ?? [];
    }

    public async Task<Style> UpsertAsync(Style style)
    {
        await UpsertAsync([style]);
        Style? updStyle = await GetAsync(style.Id);
        return updStyle ?? throw new InvalidOperationException("Style NOT found after Upsert!");
    }

    public async Task<Style[]> UpsertAsync(Style[] styles)
    {
        string sql = "REPLACE INTO beer_exchange.Styles (Id, Name, IsActive) " +
                     "VALUES (@Id, @Name, 1)";

        using IDbTransaction transaction = _connection.BeginTransaction();

        await _connection.ExecuteAsync(
            sql: sql,
            param: styles,
            transaction: transaction
        );

        transaction.Commit();

        short[] ids = styles.Select(s => s.Id).ToArray();
        return await GetAsync(ids);
    }

    public Task DeleteAsync(short id)
        => DeleteAsync([id]);

    public Task DeleteAsync(short[] ids)
    {
        string sql = "UPDATE beer_exchange.Styles " +
                     "SET IsActive = 0 " +
                     "WHERE Id IN @ids";

        return _connection.ExecuteAsync(
            sql: sql,
            param: new { ids }
        );
    }
}
