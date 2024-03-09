namespace CentricaBeerExchange.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tastings")]
public class TastingsController : ControllerBase
{
    private readonly ITastingsRepository _tastingsRepository;
    private readonly ITastingParticipantsRepository _participantsRepository;
    private readonly ITastingVotesRepository _votesRepository;

    public TastingsController(ITastingsRepository tastingsRepository, ITastingParticipantsRepository participantsRepository, ITastingVotesRepository votesRepository)
    {
        _tastingsRepository = tastingsRepository;
        _participantsRepository = participantsRepository;
        _votesRepository = votesRepository;
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
    [MinimumRole(ERole.Editor)]
    [ProducesResponseType<Dto.Tasting>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] Dto.Tasting tasting)
    {
        Tasting newTasting = await _tastingsRepository.AddAsync(tasting.Date, tasting.Theme);
        Dto.Tasting dtoNewTasting = newTasting.ToDto();

        return Ok(dtoNewTasting);
    }

    [HttpPut("{id:int}")]
    [MinimumRole(ERole.Editor)]
    [ProducesResponseType<Dto.Tasting>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Dto.Tasting tasting)
    {
        Tasting updTasting = await _tastingsRepository.UpdateAsync(id, tasting.Date, tasting.Theme);
        Dto.Tasting dtoUpdTasting = updTasting.ToDto();

        return Ok(dtoUpdTasting);
    }

    [HttpDelete("{id:int}")]
    [MinimumRole(ERole.Admin)]
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
        TastingParticipant[] participants = await _participantsRepository.GetAsync(id);
        Dto.TastingParticipant[] dtoParticipants = participants.ToDto();
        return Ok(dtoParticipants);
    }

    [HttpPost("{id:int}/register")]
    [ProducesResponseType<Dto.TastingParticipant[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterParticipationAsync([FromRoute] int id, [FromBody] Dto.TastingParticipantRegistration registration)
    {
        if (!User.IsUserIdMatchingExpected(registration.UserId))
            return BadRequest("Cannot register anyone but yourself!");

        return await AddOrUpdateParticipantsAsync(id, [registration]);
    }

    [HttpDelete("{id:int}/unregister")]
    [ProducesResponseType<Dto.TastingParticipant[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UnregisterParticipationAsync([FromRoute] int id)
    {
        if (!User.TryGetUserId(out int userId))
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not get User id!");

        return await RemoveParticipantsAsync(id, $"{userId}");
    }

    [HttpPut("{id:int}/participants")]
    [MinimumRole(ERole.Editor)]
    [ProducesResponseType<Dto.TastingParticipant[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddOrUpdateParticipantsAsync([FromRoute] int id, [FromBody] Dto.TastingParticipantRegistration[] registrations)
    {
        TastingParticipantRegistration[] domainRegistrations = registrations.ToDomain(id);
        TastingParticipant[] updParticipants = await _participantsRepository.AddOrUpdateAsync(id, domainRegistrations);
        Dto.TastingParticipant[] dtoParticipants = updParticipants.ToDto();
        return Ok(dtoParticipants);
    }

    [HttpDelete("{id:int}/participants")]
    [MinimumRole(ERole.Editor)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveParticipantsAsync([FromRoute] int id, [FromQuery] string userIds)
    {
        int[] parsedIds = userIds.SplitAndParse(int.Parse);
        await _participantsRepository.RemoveAsync(id, parsedIds);
        return Ok();
    }

    [HttpGet("{id:int}/votes")]
    [ProducesResponseType<Dto.TastingVote[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVotesAsync([FromRoute] int id)
    {
        TastingVote[] votes = await _votesRepository.GetAsync(id);
        Dto.TastingVote[] dtoVotes = votes.ToDto();
        return Ok(dtoVotes);
    }

    [HttpPut("{id:int}/vote")]
    [ProducesResponseType<Dto.TastingVote[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VoteAsync([FromRoute] int id, [FromBody] Dto.TastingVoteRegistration registration)
    {
        if (!User.IsUserIdMatchingExpected(registration.UserId))
            return BadRequest("Cannot vote on someone elses behalf!");

        if (User.IsUserIdMatchingExpected(registration.VotedUserId))
            return BadRequest("Cannot vote for yourself!");

        return await AddOrUpdateVotesAsync(id, [registration]);
    }

    [HttpPut("{id:int}/votes")]
    [MinimumRole(ERole.Editor)]
    [ProducesResponseType<Dto.TastingVote[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddOrUpdateVotesAsync([FromRoute] int id, [FromBody] Dto.TastingVoteRegistration[] registrations)
    {
        TastingVoteRegistration[] domainRegistrations = registrations.ToDomain(id);
        TastingVote[] updVotes = await _votesRepository.AddOrUpdateAsync(id, domainRegistrations);
        Dto.TastingVote[] dtoUpdVotes = updVotes.ToDto();
        return Ok(dtoUpdVotes);
    }

    [HttpDelete("{id:int}/votes")]
    [MinimumRole(ERole.Editor)]
    [ProducesResponseType<Dto.TastingVote[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemovedVotesAsync([FromRoute] int id, [FromQuery] string userIds)
    {
        int[] parsedIds = userIds.SplitAndParse(int.Parse);
        await _votesRepository.RemoveAsync(id, parsedIds);
        return Ok();
    }
}
