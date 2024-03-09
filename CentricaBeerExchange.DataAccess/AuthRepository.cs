namespace CentricaBeerExchange.DataAccess;

public class AuthRepository : IAuthRepository
{
    const int VERIFICATION_CODE_VALID_MINUTES = 15;
    const ERole DEFAULT_ROLE = ERole.User;

    private readonly ITimeProvider _timeProvider;
    private readonly IDbConnection _connection;

    public AuthRepository(ITimeProvider timeProvider, IDbConnection connection)
    {
        _timeProvider = timeProvider;
        _connection = connection;
    }

    public async Task UpsertVerificationCodeAsync(string email, int code)
    {
        string sql = "REPLACE INTO beer_exchange.Verification (Email, CodeHash, CreatedAtUtc, ValidUntilUtc) " +
                     "VALUES (@email, @codeHash, @createdAtUtc, @validUntilUtc)";

        int changed = await _connection.ExecuteAsync(
            sql: sql,
            param: new
            {
                email,
                codeHash = CryptoHelper.GetHash(code),
                createdAtUtc = _timeProvider.UtcNow,
                validUntilUtc = _timeProvider.UtcNow.AddMinutes(VERIFICATION_CODE_VALID_MINUTES)
            }
        );

        if (changed < 1)
            throw new InvalidOperationException($"Verification Code was not written to DB!");
    }

    public async Task<Verification?> GetVerificationAsync(string email)
    {
        string sql = "SELECT Email, CodeHash, ValidUntilUtc " +
                     "FROM beer_exchange.Verification " +
                     "WHERE Email = @email";

        Verification? verification = await _connection.QuerySingleOrDefaultAsync<Verification>(
            sql: sql,
            param: new { email }
        );

        return verification;
    }

    public async Task RemoveVerificationAsync(string email)
    {
        string sql = "DELETE FROM beer_exchange.Verification " +
                     "WHERE Email = @email";

        int deleted = await _connection.ExecuteAsync(
            sql: sql,
            param: new { email }
        );

        if (deleted < 1)
            throw new InvalidOperationException($"Verification Code was not removed from DB!");
    }

    public async Task<User> GetOrCreateUserAsync(string email)
    {
        string sqlSelect = "SELECT Id, Email, Role, Name " +
                           "FROM beer_exchange.Users " +
                           "WHERE Email = @email";

        User? user = await _connection.QuerySingleOrDefaultAsync<User>(
            sql: sqlSelect,
            param: new { email }
        );

        if (user is not null)
            return user;

        string sqlInsert = "INSERT INTO (Email, Role) " +
                           "VALUES (@email, @role); " +
                           "SELECT LAST_INSERT_ID();";

        int? userId = await _connection.QuerySingleOrDefaultAsync<int>(
             sql: sqlInsert,
             param: new { email, role = DEFAULT_ROLE }
        );

        if (userId is null)
            throw new InvalidOperationException($"User for '{email}' was NOT created!");

        return await GetUserAsync(userId.Value);
    }

    public async Task<User> GetUserAsync(int id)
    {
        string sqlSelect = "SELECT Id, Email, Role " +
                           "FROM beer_exchange.Users " +
                           "WHERE Id = @id";

        User? user = await _connection.QuerySingleOrDefaultAsync<User>(
            sql: sqlSelect,
            param: new { id }
        );

        return user ?? throw new InvalidOperationException($"User with Id '{id}' was NOT found!");
    }

    public async Task<User> UpdateUserEmailAsync(int id, string newEmail)
    {
        string sql = "UPDATE beer_exchange.Users " +
                     "SET Email = @email " +
                     "WHERE UserId = @userId";

        await _connection.ExecuteAsync(
            sql: sql,
            param: new { userId = id, email = newEmail }
        );

        return await GetUserAsync(id);
    }

    public async Task<TokenDetails?> GetTokenAsync(Guid tokenId)
    {
        string sql = "SELECT UserId, TokenId, TokenExpiryUtc, RefreshToken, RefreshExpiryUtc " +
                     "FROM beer_exchange.TokenDetails " +
                     "WHERE TokenId = @tokenId";

        TokenDetails? tokenDetails = await _connection.QuerySingleOrDefaultAsync<TokenDetails>(
            sql: sql,
            param: new { tokenId }
        );

        return tokenDetails;
    }

    public async Task UpsertTokenAsync(TokenDetails tokenDetails)
    {
        string sql = "REPLACE INTO beer_exchange.TokenDetails (UserId, TokenId, TokenExpiryUtc, RefreshToken, RefreshExpiryUtc) " +
                     "VALUES (@UserId, @TokenId, @TokenExpiryUtc, @RefreshToken, @RefreshExpiryUtc)";

        int changed = await _connection.ExecuteAsync(
            sql: sql,
            param: tokenDetails
        );

        if (changed < 1)
            throw new InvalidOperationException("Token Details were not written to DB!");
    }

    public async Task<bool> RemoveTokenAsync(int userId)
    {
        string sql = "DELETE FROM beer_exchange.TokenDetails " +
                     "WHERE UserId = @userId";

        int changed = await _connection.ExecuteAsync(
            sql: sql,
            param: new { userId }
        );

        bool wasRemoved = changed >= 1;
        return wasRemoved;
    }
}
