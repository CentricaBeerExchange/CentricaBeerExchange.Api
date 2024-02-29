namespace CentricaBeerExchange.Domain.Models;

public record TastingVote(int TastingId, int UserId, string UserName, int VotedUserId, string VotedUserName);
