namespace CentricaBeerExchange.DataAccess.MongoDb.Queries;

internal static class UserQueries
{
    const ERole DEFAULT_ROLE = ERole.User;

    public static FilterDefinition<DbUser> FilterNone()
        => Builders<DbUser>.Filter.Empty;

    public static FilterDefinition<DbUser> FilterById(int id)
        => Builders<DbUser>.Filter.Eq(u => u.Id, id.ToGuid());

    public static FilterDefinition<DbUser> FilterByEmail(string email)
        => Builders<DbUser>.Filter.Eq(u => u.Email, email);

    public static SortDefinition<DbUser> SortByCreatedDescending
        => Builders<DbUser>.Sort.Descending(u => u.CreatedAtUtc);

    public static FindOptions<DbUser, DbUser> OptionsSortByCreatedDescending(int? limit = null)
        => new()
        {
            Sort = SortByCreatedDescending,
            Limit = limit
        };

    public static UpdateDefinition<DbUser> Upsert(int id, string email, DateTime utcNow, string? name = null, ERole role = DEFAULT_ROLE)
        => Builders<DbUser>.Update
            .SetOnInsert(u => u.Id, id.ToGuid())
            .SetOnInsert(u => u.Email, email)
            .SetOnInsert(u => u.CreatedAtUtc, utcNow)
            .Set(u => u.Name, name)
            .Set(u => u.Role, role);
}
