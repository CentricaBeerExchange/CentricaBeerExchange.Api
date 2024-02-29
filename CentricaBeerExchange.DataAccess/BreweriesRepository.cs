namespace CentricaBeerExchange.DataAccess;

public class BreweriesRepository : IBreweriesRepository
{
    private readonly IDbConnection _connection;

    public BreweriesRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Brewery[]> GetAsync()
    {
        string sql = "SELECT Id, Name, UntappdId, Location, Type, Thumbnail " +
                     "FROM beer_exchange.Breweries";

        IEnumerable<Brewery> query = await _connection.QueryAsync<Brewery>(sql);

        return query?.ToArray() ?? [];
    }
    public async Task<Brewery?> GetAsync(int id)
    {
        string sql = "SELECT Id, Name, UntappdId, Location, Type, Thumbnail " +
                     "FROM beer_exchange.Breweries " +
                     "WHERE Id = @id";

        Brewery? brewery = await _connection.QuerySingleOrDefaultAsync<Brewery>(
            sql: sql,
            param: new { id }
        );

        return brewery;
    }

    public async Task<Brewery> AddAsync(string name, string? untappdId, string? location, string? type, string? thumbnail)
    {
        if (await ExistsAsync(name, untappdId))
            throw new InvalidOperationException("Brewery with that Name or Untappd Id already exists!");

        string sql = "INSERT INTO beer_exchange.Breweries (Name, UntappdId, Location, Type, Thumbnail) " +
                     "VALUES (@name, @untappdId, @location, @type, @thumbnail); " +
                     "SELECT LAST_INSERT_ID();";

        int insertedId = await _connection.QuerySingleAsync<int>(
            sql: sql,
            param: new { name, untappdId, location, type, thumbnail }
        );

        Brewery? brewery = await GetAsync(insertedId);
        return brewery ?? throw new InvalidOperationException("Brewery NOT found after Insert!");
    }

    public async Task<Brewery> UpdateAsync(int id, string name, string? untappdId, string? location, string? type, string? thumbnail)
    {
        if (await ExistsAsync(name, untappdId, id))
            throw new InvalidOperationException("Another Brewery with that Name or Untappd Id already exists!");

        string sql = "UPDATE beer_exchange.Breweries " +
                     "SET Name = @name, UntappdId = @untappdId, Location = @location, Type = @type, Thumbnail = @thumbnail " +
                     "WHERE Id = @id";

        await _connection.ExecuteAsync(
            sql: sql,
            param: new { id, name, untappdId, location, type, thumbnail }
        );

        Brewery? brewery = await GetAsync(id);
        return brewery ?? throw new InvalidOperationException("Brewery NOT found after Update!");
    }

    public Task DeleteAsync(int id)
    {
        string sql = "DELETE FROM beer_exchange.Breweries " +
                     "WHERE Id = @id";

        return _connection.ExecuteAsync(
            sql: sql,
            param: new { id }
        );
    }

    private async Task<bool> ExistsAsync(string? name, string? untappdId, int? idToIgnore = null)
    {
        if (name is null && untappdId is null)
            return false;

        List<string> clauses = [];

        if (name is not null)
            clauses.Add("Name = @name");
        if (untappdId is not null)
            clauses.Add("UntappdId = @untappdId");

        string subSql = "SELECT * " +
                        "FROM beer_exchange.Breweries " +
                       $"WHERE ({string.Join(" OR ", clauses)})";

        if (idToIgnore.HasValue)
            subSql = $"{subSql} AND Id != @idToIgnore";

        string sql = $"SELECT EXISTS({subSql})";

        bool exists = await _connection.QuerySingleOrDefaultAsync<bool>(
            sql: sql,
            param: new { name, untappdId, idToIgnore }
        );

        return exists;
    }
}
