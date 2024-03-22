
namespace CentricaBeerExchange.DataAccess.MongoDb;

public class MongoDbProfileRepository : IProfileRepository
{
    public Task<Profile[]> GetAsync() => throw new NotImplementedException();
    public Task<Profile?> GetAsync(int id) => throw new NotImplementedException();
    public Task<Profile?> UpdateAsync(int id, Profile updatedProfile) => throw new NotImplementedException();
}
