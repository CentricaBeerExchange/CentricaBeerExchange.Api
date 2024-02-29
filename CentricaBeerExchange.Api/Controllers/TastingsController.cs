namespace CentricaBeerExchange.Api.Controllers;

[ApiController]
[Route("api/tastings")]
public class TastingsController : ControllerBase
{
    private readonly ITastingsRepository _tastingsRepository;
    private readonly ITastingParticipantsRepository _tastingParticipantsRepository;

    public TastingsController(ITastingsRepository tastingsRepository, ITastingParticipantsRepository tastingParticipantsRepository)
    {
        _tastingsRepository = tastingsRepository;
        _tastingParticipantsRepository = tastingParticipantsRepository;
    }

    [HttpGet("")]
    [ProducesResponseType<Dto.Tasting[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        Tasting[] tastings = await _tastingsRepository.GetAsync();
        Dto.Tasting[] dtoTastings = tastings.ToDto();

        return Ok(dtoTastings);
    }

    [HttpPost("")]
    [ProducesResponseType<Dto.Tasting>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] Dto.Tasting tasting)
    {
        Tasting newTasting = await _tastingsRepository.AddAsync(tasting.Date, tasting.Theme);
        Dto.Tasting dtoNewTasting = newTasting.ToDto();

        return Ok(dtoNewTasting);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<Dto.Tasting>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Dto.Tasting tasting)
    {
        Tasting updTasting = await _tastingsRepository.UpdateAsync(id, tasting.Date, tasting.Theme);
        Dto.Tasting dtoUpdTasting = updTasting.ToDto();

        return Ok(dtoUpdTasting);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _tastingsRepository.DeleteAsync(id);
        return Ok();
    }

    [HttpGet("{id:int}/participants")]
    [ProducesResponseType<Dto.TastingParticipant[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetParticipantsAsync([FromRoute] int id)
    {
        TastingParticipant[] participants = await _tastingParticipantsRepository.GetAsync(id);
        Dto.TastingParticipant[] dtoParticipants = participants.ToDto();
        return Ok(dtoParticipants);
    }

    [HttpPost("{id:int}/participants")]
    [ProducesResponseType<Dto.TastingParticipant[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddOrUpdateParticipantsAsync([FromRoute] int id, [FromBody] Dto.TastingParticipantRegistration[] registrations)
    {
        TastingParticipantRegistration[] domainRegistrations = registrations.ToDomain(id);
        TastingParticipant[] updParticipants = await _tastingParticipantsRepository.AddOrUpdateAsync(id, domainRegistrations);
        Dto.TastingParticipant[] dtoParticipants = updParticipants.ToDto();
        return Ok(dtoParticipants);
    }

    [HttpDelete("{id:int}/participants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveParticipantsAsync([FromRoute] int id, [FromQuery] string userIds)
    {
        int[] parsedIds = userIds.SplitAndParse(int.Parse);
        await _tastingParticipantsRepository.RemoveAsync(id, parsedIds);
        return Ok();
    }
}
