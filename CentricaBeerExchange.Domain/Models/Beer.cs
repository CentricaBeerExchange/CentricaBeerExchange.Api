namespace CentricaBeerExchange.Domain.Models;

public record Beer(int Id, string Name, BreweryMeta Brewery, Style Style, decimal? Rating, decimal? ABV, int? UntappdId);
