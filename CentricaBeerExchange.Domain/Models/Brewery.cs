namespace CentricaBeerExchange.Domain.Models;

public record Brewery(int Id, string Name, string? UntappdId, string? Location, string? Type, string? Thumbnail);

public record BreweryMeta(int Id, string Name);
