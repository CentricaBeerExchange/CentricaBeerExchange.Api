namespace CentricaBeerExchange.DataAccess;

public class ProfileRepository : IProfileRepository
{
    private readonly IDbConnection _connection;

    public ProfileRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Profile[]> GetAsync()
    {
        string sql = "SELECT Id, Email, Name, Department " +
                     "FROM beer_exchange.Users";

        IEnumerable<Profile> query = await _connection.QueryAsync<Profile>(sql);

        return query?.ToArray() ?? [];
    }

    public async Task<Profile?> GetAsync(int userId)
    {
        string sql = "SELECT Id, Email, Name, Department " +
                     "FROM beer_exchange.Users " +
                     "WHERE Id = @userId";

        Profile? profile = await _connection.QuerySingleOrDefaultAsync<Profile>(
            sql: sql,
            param: new { userId }
        );

        return profile;
    }

    public async Task<Profile?> UpdateAsync(int userId, string updatedName, string updatedDepartment)
    {
        string sql = "UPDATE beer_exchange.Users " +
                     "SET Name = @updatedName, Department = @updatedDepartment " +
                     "WHERE Id = @userId";

        int updated = await _connection.ExecuteAsync(
            sql: sql,
            param: new { userId, updatedName, updatedDepartment }
        );

        if (updated < 1)
            return null;

        return await GetAsync(userId);
    }
}
