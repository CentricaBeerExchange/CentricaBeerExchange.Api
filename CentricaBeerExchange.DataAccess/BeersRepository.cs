namespace CentricaBeerExchange.DataAccess;

public class BeersRepository : IBeersRepository
{
    private readonly IDbConnection _connection;

    public BeersRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Beer[]> GetAsync()
    {
        IEnumerable<DbBeer> query = await _connection.QueryAsync<DbBeer>(sql: SQL_SELECT_ALL);

        Beer[] beers = query?.Select(Map).ToArray() ?? [];

        return beers;
    }

    public async Task<Beer[]> GetAsync(int[] ids)
    {
        IEnumerable<DbBeer> query = await _connection.QueryAsync<DbBeer>(
            sql: SQL_SELECT_MANY,
            param: new { ids }
        );

        Beer[] beers = query?.Select(Map).ToArray() ?? [];

        return beers;
    }

    public async Task<Beer?> GetAsync(int id)
    {
        DbBeer? dbBeer = await _connection.QuerySingleOrDefaultAsync<DbBeer>(
            sql: SQL_SELECT_SINGLE,
            param: new { id }
        );

        if (dbBeer is null)
            return null;

        return Map(dbBeer);
    }

    public async Task<Beer> AddAsync(string name, int breweryId, short styleId, decimal? rating, decimal? abv, int? untappdId)
    {
        if (await ExistsAsync(untappdId))
            throw new InvalidOperationException("Another Beer with that Untappd Id already exists!");

        int insertedId = await _connection.QuerySingleAsync<int>(
            sql: SQL_INSERT,
            param: new { name, breweryId, styleId, rating, abv, untappdId }
        );

        Beer? beer = await GetAsync(insertedId);
        return beer ?? throw new InvalidOperationException("Beer NOT found after Insert!");
    }

    public async Task<Beer> UpdateAsync(int id, string name, int breweryId, short styleId, decimal? rating, decimal? abv, int? untappdId)
    {
        if (await ExistsAsync(untappdId, id))
            throw new InvalidOperationException("Another Beer with that Untappd Id already exists!");

        await _connection.ExecuteAsync(
            sql: SQL_UPDATE,
            param: new { name, breweryId, styleId, rating, abv, untappdId }
        );

        Beer? beer = await GetAsync(id);
        return beer ?? throw new InvalidOperationException("Beer NOT found after Update!");
    }

    public Task DeleteAsync(int id)
    {
        return _connection.ExecuteAsync(
            sql: SQL_DELETE,
            param: new { id }
        );
    }

    private async Task<bool> ExistsAsync(int? untappdId, int? idToIgnore = null)
    {
        if (untappdId is null)
            return false;

        string subSql = "SELECT * FROM beer_exchange.Beers " +
                        "WHERE UntappdId = @untappdId";

        if (idToIgnore is not null)
            subSql = $"{subSql} AND Id != @idToIgnore";

        string sql = $"SELECT EXISTS({subSql})";

        bool exists = await _connection.QuerySingleOrDefaultAsync<bool>(
            sql: sql,
            param: new { untappdId, idToIgnore }
        );

        return exists;
    }

    static Beer Map(DbBeer beer)
        => new(
            Id: beer.Id,
            Name: beer.Name,
            Brewery: new BreweryMeta(beer.BreweryId, beer.BreweryName),
            Style: new Style(beer.StyleId, beer.StyleName),
            Rating: beer.Rating,
            ABV: beer.ABV,
            UntappdId: beer.UntappdId
        );

    internal record DbBeer(
        int Id,
        string Name,
        int BreweryId,
        string BreweryName,
        short StyleId,
        string StyleName,
        decimal? Rating,
        decimal? ABV,
        int? UntappdId
    );

    const string SQL_SELECT_BASE =
@"SELECT 
	beer.Id 		AS Id,
    beer.Name 		AS Name,
    beer.Brewery 	AS BreweryId,
    brew.Name		AS BreweryName,
    beer.Style 		AS StyleId,
    style.Name		AS StyleName,
    beer.Rating 	AS Rating,
    beer.ABV 		AS ABV,
    beer.UntappdId 	AS UntappdId
FROM 
	beer_exchange.Beers beer
    INNER JOIN beer_exchange.Breweries brew
		ON brew.Id = beer.Brewery
	INNER JOIN beer_exchange.Styles style
		ON style.Id = beer.Style";

    const string SQL_SELECT_ALL = 
$"{SQL_SELECT_BASE};";

    const string SQL_SELECT_MANY =
$@"{SQL_SELECT_BASE}
WHERE beer.Id IN @ids;";

    const string SQL_SELECT_SINGLE =
$@"{SQL_SELECT_BASE}
WHERE beer.Id = @id;";

    const string SQL_INSERT =
@"INSERT INTO beer_exchange.Beers (Name, Brewery, Style, Rating, ABV, UntappdId)
VALUES (@name, @breweryId, @styleId, @rating, @abv, @untappdId);
SELECT LAST_INSERT_ID();";

    const string SQL_UPDATE =
@"UPDATE beer_exchange.Beers
SET Name = @name, Brewery = @breweryId, Style = @styleId, Rating = @rating, ABV = @abv, UntappdId = @untappdId
WHERE Id = @id";

    const string SQL_DELETE =
@"DELETE FROM beer_exchange.Beers
WHERE Id = @id";
}
