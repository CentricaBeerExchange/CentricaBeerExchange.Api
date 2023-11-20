using CentricaBeerExchange.Domain.Models;

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

    public async Task<Profile?> UpdateAsync(int userId, Profile updatedProfile)
    {
        string sql = "UPDATE beer_exchange.Users " +
                     "SET Name = @Name, Department = @Department, @Thumbnail" +
                     "WHERE Id = @userId";

        Profile? afterUpdate = await CombineProfilesAsync(userId, updatedProfile);

        if (afterUpdate is null)
            return null;

        int updated = await _connection.ExecuteAsync(
            sql: sql,
            param: new
            {
                userId,
                afterUpdate.Name,
                afterUpdate.Department,
                afterUpdate.Thumbnail
            }
        );

        if (updated < 1)
            return null;

        return afterUpdate;
    }

    private async Task<Profile?> CombineProfilesAsync(int userId, Profile updatedProfile)
    {
        Profile? existing = await GetAsync(userId);

        if (existing is null)
            return null;

        return new Profile(
            id: userId,
            email: existing.Email,
            name: updatedProfile.Name ?? existing.Name,
            department: updatedProfile.Department ?? existing.Department,
            thumbnail: updatedProfile.Thumbnail ?? existing.Thumbnail
        );
    }
}
