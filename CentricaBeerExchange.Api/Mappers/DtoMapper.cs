namespace CentricaBeerExchange.Api.Mappers;

public static class DtoMapper
{
    public static Dto.Beer[] ToDto(this IEnumerable<Beer> beers)
        => beers?.Select(ToDto).ToArray() ?? [];

    public static Dto.Beer ToDto(this Beer beer)
        => new(
            id: beer.Id,
            name: beer.Name,
            brewery: ToDto(beer.Brewery),
            style: ToDto(beer.Style),
            rating: beer.Rating,
            abv: beer.ABV,
            untappdId: beer.UntappdId
        );

    public static Dto.Brewery[] ToDto(this IEnumerable<Brewery> breweries)
        => breweries?.Select(ToDto).ToArray() ?? [];

    public static Dto.Brewery ToDto(this Brewery brewery)
        => new(
            id: brewery.Id,
            name: brewery.Name,
            untappdId: brewery.UntappdId,
            location: brewery.Location,
            type: brewery.Type,
            thumbnail: brewery.Thumbnail
        );

    public static Dto.BreweryMeta ToDto(this BreweryMeta brewery)
        => new(
            id: brewery.Id,
            name: brewery.Name
        );

    public static Dto.Style[] ToDto(this IEnumerable<Style> styles)
        => styles?.Select(ToDto).ToArray() ?? [];

    public static Dto.Style ToDto(this Style style)
        => new(
            id: style.Id,
            name: style.Name
        );

    public static Dto.Tasting[] ToDto(this IEnumerable<Tasting> tastings)
        => tastings?.Select(ToDto).ToArray() ?? [];

    public static Dto.Tasting ToDto(this Tasting tasting)
        => new(
            id: tasting.Id,
            date: tasting.Date,
            theme: tasting.Theme
        );
}
