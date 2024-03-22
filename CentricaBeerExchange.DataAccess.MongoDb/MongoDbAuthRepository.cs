namespace CentricaBeerExchange.DataAccess.MongoDb;

public class MongoDbAuthRepository : IAuthRepository
{
    private readonly ITimeProvider _timeProvider;
    private readonly IMongoClient _client;

    private readonly IMongoCollection<DbVerification> _verificationCollection;
    private readonly IMongoCollection<DbUser> _userCollection;
    private readonly IMongoCollection<DbTokenDetails> _tokenDetailsCollection;

    public MongoDbAuthRepository(ITimeProvider timeProvider, IMongoClient client, IMongoDatabase database)
    {
        _timeProvider = timeProvider;
        _client = client;
        _verificationCollection = database.GetCollection<DbVerification>(nameof(Verification));
        _userCollection = database.GetCollection<DbUser>(nameof(User));
        _tokenDetailsCollection = database.GetCollection<DbTokenDetails>(nameof(TokenDetails));
    }

    public async Task UpsertVerificationCodeAsync(string email, int code)
    {
        UpdateResult result = await _verificationCollection.UpdateOneAsync(
            filter: VerificationQueries.FilterByEmail(email),
            update: VerificationQueries.Upsert(email, code, _timeProvider.UtcNow),
            options: QueryOptions.UpsertOptions
        );

        if (!result.IsAcknowledged || result.MatchedCount > 0 && result.ModifiedCount < 1)
            throw new InvalidOperationException($"Verification Code was not written to DB!");
    }

    public async Task<Verification?> GetVerificationAsync(string email)
    {
        IAsyncCursor<DbVerification> cursor = await _verificationCollection.FindAsync(
            filter: VerificationQueries.FilterByEmail(email)
        );

        DbVerification? dbVerification = await cursor.SingleOrDefaultAsync();
        return dbVerification?.Map();
    }

    public async Task RemoveVerificationAsync(string email)
    {
        DeleteResult result = await _verificationCollection.DeleteOneAsync(
            filter: VerificationQueries.FilterByEmail(email)
        );

        if (result.DeletedCount > 1)
            throw new InvalidOperationException($"Multiple elements deleted from DB!");
    }

    public async Task<User> GetOrCreateUserAsync(string email)
    {
        using IClientSessionHandle sessionHandle = await _client.StartSessionAsync();

        try
        {
            sessionHandle.StartTransaction();

            IAsyncCursor<DbUser> cursor = await _userCollection.FindAsync(
                session: sessionHandle,
                filter: UserQueries.FilterByEmail(email)
            );

            DbUser? user = await cursor.SingleOrDefaultAsync();

            if (user is not null)
                return user.Map();

            int maxUserId = await GetMaxUserIdAsync(sessionHandle);
            int userId = maxUserId + 1;

            await _userCollection.UpdateOneAsync(
                filter: UserQueries.FilterByEmail(email),
                update: UserQueries.Upsert(userId, email, _timeProvider.UtcNow),
                options: QueryOptions.UpsertOptions
            );

            await sessionHandle.CommitTransactionAsync();

            return await GetUserAsync(userId);
        }
        catch (Exception ex)
        {
            await sessionHandle.AbortTransactionAsync();
            throw;
        }
    }

    public async Task<User> GetUserAsync(int id)
    {
        IAsyncCursor<DbUser> cursor = await _userCollection.FindAsync(
            filter: UserQueries.FilterById(id)
        );

        DbUser? user = await cursor.SingleOrDefaultAsync();
        return user?.Map() ?? throw new InvalidOperationException($"User with Id '{id}' was NOT found!");
    }

    public async Task<TokenDetails?> GetTokenAsync(Guid tokenId)
    {
        IAsyncCursor<DbTokenDetails> cursor = await _tokenDetailsCollection.FindAsync(
            filter: Builders<DbTokenDetails>.Filter.Eq(t => t.TokenId, tokenId)
        );

        DbTokenDetails? token = await cursor.SingleOrDefaultAsync();
        return token?.Map();
    }

    public async Task UpsertTokenAsync(TokenDetails tokenDetails)
    {
        UpdateResult result = await _tokenDetailsCollection.UpdateOneAsync(
            filter: TokenQueries.FilterByUserId(tokenDetails.UserId),
            update: TokenQueries.Upsert(tokenDetails),
            options: QueryOptions.UpsertOptions
        );

        if (!result.IsAcknowledged || result.MatchedCount > 0 && result.ModifiedCount < 1)
            throw new InvalidOperationException("Token Details were not written to DB!");

    }

    public async Task<bool> RemoveTokenAsync(int userId)
    {
        FilterDefinition<DbTokenDetails> filterById = Builders<DbTokenDetails>.Filter.Eq(t => t.UserId, userId);
        DeleteResult result = await _tokenDetailsCollection.DeleteOneAsync(filterById);
        return result.DeletedCount >= 1;
    }

    private async Task<int> GetMaxUserIdAsync(IClientSessionHandle sessionHandle)
    {
        IAsyncCursor<DbUser> cursor = await _userCollection.FindAsync(
            session: sessionHandle,
            filter: UserQueries.FilterNone(),
            options: UserQueries.OptionsSortByCreatedDescending(limit: 1)
        );

        DbUser? lastUser = await cursor.SingleOrDefaultAsync();

        return lastUser?.Id.ToInt() ?? 0;
    }
}
