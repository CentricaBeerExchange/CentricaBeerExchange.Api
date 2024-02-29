namespace CentricaBeerExchange.Domain.Models.Auth;

public record Verification(string Email, string CodeHash, DateTime ValidUntilUtc);
