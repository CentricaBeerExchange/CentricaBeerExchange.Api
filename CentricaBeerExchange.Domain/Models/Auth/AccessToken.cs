namespace CentricaBeerExchange.Domain.Models.Auth;

public record AccessToken(string Token, DateTime ExpiresAtUtc);
