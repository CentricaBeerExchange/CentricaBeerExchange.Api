namespace CentricaBeerExchange.Domain.Repositories;

public interface ITastingParticipantsRepository
{
    Task<TastingParticipant[]> GetAsync(int tastingId, int? userId = null);
    Task<TastingParticipant[]> AddOrUpdateAsync(int tastingId, TastingParticipantRegistration[] registrations);
    Task<TastingParticipant> ChangeBeerAsync(int tastingId, int userId, int newBeerId);
    Task<TastingParticipant[]> RemoveAsync(int tastingId, int[] userIds);
}
