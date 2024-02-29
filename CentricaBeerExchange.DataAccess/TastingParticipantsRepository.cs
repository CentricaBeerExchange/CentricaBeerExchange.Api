namespace CentricaBeerExchange.DataAccess;

public class TastingParticipantsRepository : ITastingParticipantsRepository
{
    private readonly IDbConnection _connection;
    private readonly IBeersRepository _beersRepository;

    public TastingParticipantsRepository(IDbConnection connection, IBeersRepository beersRepository)
    {
        _connection = connection;
        _beersRepository = beersRepository;
    }

    public async Task<TastingParticipant[]> GetAsync(int tastingId, int? userId = null)
    {
        string sql = "SELECT u.Id, u.Name, t.BeerId " +
                     "FROM beer_exchange.TastingParticipants t " +
                     "INNER JOIN beer_exchange.Users u ON u.Id = t.UserId " +
                     "WHERE t.TastingId = @tastingId ";

        if (userId is not null)
            sql = $"{sql} AND t.UserId = @userId";

        IEnumerable<DbTastingParticipant> query = await _connection.QueryAsync<DbTastingParticipant>(
            sql: sql,
            param: new { tastingId, userId }
        );

        if (query is null)
            return [];

        int[] beerIds = [.. query.Select(p => p.BeerId)];

        Beer[] beers = await _beersRepository.GetAsync(beerIds);
        Dictionary<int, Beer> lookup = beers.ToDictionary(b => b.Id);

        TastingParticipant[] participants = query.Select(db => new TastingParticipant(db.Id, db.Name, lookup[db.BeerId])).ToArray();

        return participants;
    }

    public async Task<TastingParticipant[]> AddOrUpdateAsync(int tastingId, TastingParticipantRegistration[] registrations)
    {
        string sql = "REPLACE INTO beer_exchange.TastingParticipants (TastingId, UserId, BeerId) " +
                     "VALUES (@TastingId, @UserId, @BeerId)";

        await _connection.ExecuteAsync(
            sql: sql,
            param: registrations
        );

        TastingParticipant[] participants = await GetAsync(tastingId);
        return participants;
    }

    public async Task<TastingParticipant> ChangeBeerAsync(int tastingId, int userId, int newBeerId)
    {
        string sql = "UPDATE beer_exchange.TastingParticipants " +
                     "SET BeerId = @newBeerId " +
                     "WHERE TastingId = @tastingId AND UserId = @userId";

        await _connection.ExecuteAsync(
            sql: sql,
            param: new { tastingId, userId, newBeerId }
        );

        TastingParticipant[] participants = await GetAsync(tastingId, userId);
        return participants?.FirstOrDefault() ?? throw new InvalidOperationException("Participant not found after update!");
    }

    public async Task<TastingParticipant[]> RemoveAsync(int tastingId, int[] userIds)
    {
        string sql = "DELETE FROM beer_exchange.TastingParticipants " +
                     "WHERE TastingId = @tastingId AND UserId IN @userIds";

        await _connection.ExecuteAsync(
            sql: sql,
            param: new { tastingId, userIds }
        );

        TastingParticipant[] participants = await GetAsync(tastingId);
        return participants;
    }

    private record DbTastingParticipant(int Id, string Name, int BeerId);
}
