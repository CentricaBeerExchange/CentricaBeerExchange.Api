namespace CentricaBeerExchange.Api.Mappers;

public static class DomainMapper
{
    public static Style[] ToDomain(this IEnumerable<Dto.Style> dtoStyles)
        => dtoStyles?.Select(ToDomain).ToArray() ?? [];

    public static Style ToDomain(this Dto.Style dtoStyle)
        => new(
            Id: dtoStyle.Id,
            Name: dtoStyle.Name
        );
}
