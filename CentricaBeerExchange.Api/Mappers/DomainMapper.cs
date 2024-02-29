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

    public static TastingParticipantRegistration[] ToDomain(this Dto.TastingParticipantRegistration[] dtoRegs, int tastingId)
        => dtoRegs?.Select(ToDomain).ToArray() ?? [];

    public static TastingParticipantRegistration ToDomain(this Dto.TastingParticipantRegistration dtoReg, int tastingId)
        => new(
            TastingId: tastingId, 
            UserId: dtoReg.UserId, 
            BeerId: dtoReg.BeerId
        );
}
