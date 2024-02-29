
namespace CentricaBeerExchange.Domain.Repositories;

public interface ITastingVotesRepository
{
    Task<TastingVote[]> GetAsync(int tastingId);
    Task<TastingVote[]> AddOrUpdateAsync(int tastingId, TastingVoteRegistration[] registrations);
    Task<TastingVote[]> RemoveAsync(int tastingId, int[] userIds);
}
