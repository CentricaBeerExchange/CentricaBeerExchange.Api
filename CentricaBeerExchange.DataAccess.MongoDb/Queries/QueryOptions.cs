namespace CentricaBeerExchange.DataAccess.MongoDb.Queries;

internal static class QueryOptions
{
    public static readonly UpdateOptions UpsertOptions = new() { IsUpsert = true };
}
