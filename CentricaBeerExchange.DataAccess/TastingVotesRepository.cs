namespace CentricaBeerExchange.DataAccess;

public class TastingVotesRepository : ITastingVotesRepository
{
    private readonly IDbConnection _connection;

    public TastingVotesRepository(IDbConnection connection)
    {
        _connection = connection;
        _connection.Open();
    }

    public async Task<TastingVote[]> GetAsync(int tastingId)
    {
        string sql = "SELECT t.TastingId, t.UserId, u.Name AS UserName, t.VotedUserId, v.Name AS VotedUserName " +
                     "FROM beer_exchange.TastingVotes t " +
                     "INNER JOIN beer_exchange.Users u ON u.Id = t.UserId " +
                     "INNER JOIN beer_exchange.Users v ON v.Id = t.VotedUserId " +
                     "WHERE t.TastingId = @tastingId";

        IEnumerable<TastingVote> query = await _connection.QueryAsync<TastingVote>(
            sql: sql,
            param: new { tastingId }
        );

        return query?.ToArray() ?? [];
    }

    public async Task<TastingVote[]> AddOrUpdateAsync(int tastingId, TastingVoteRegistration[] registrations)
    {
        string sql = "REPLACE INTO beer_exchange.TastingVotes (TastingId, UserId, VotedUserId) " +
                     "VALUES (@TastingId, @UserId, @VotedUserId)";

        using IDbTransaction transaction = _connection.BeginTransaction();

        await _connection.ExecuteAsync(
            sql: sql,
            param: registrations,
            transaction: transaction
        );

        transaction.Commit();

        TastingVote[] votes = await GetAsync(tastingId);
        return votes;
    }

    public async Task<TastingVote[]> RemoveAsync(int tastingId, int[] userIds)
    {
        string sql = "DELETE FROM beer_exchange.TastingVotes " +
                     "WHERE TastingId = @tastingId AND UserId IN @userIds";

        await _connection.ExecuteAsync(
            sql: sql,
            param: new { tastingId, userIds }
        );

        TastingVote[] votes = await GetAsync(tastingId);
        return votes;
    }
}
