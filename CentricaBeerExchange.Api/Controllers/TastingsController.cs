namespace CentricaBeerExchange.Api.Controllers;

[ApiController]
[Route("api/tastings")]
public class TastingsController : ControllerBase
{
    private readonly ITastingsRepository _tastingsRepository;

    public TastingsController(ITastingsRepository tastingsRepository)
    {
        _tastingsRepository = tastingsRepository;
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
    public async Task<IActionResult> PostAsync(Dto.Tasting tasting)
    {
        Tasting newTasting = await _tastingsRepository.AddAsync(tasting.Date, tasting.Theme);
        Dto.Tasting dtoNewTasting = newTasting.ToDto();

        return Ok(dtoNewTasting);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<Dto.Tasting>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PutAsync(int id, Dto.Tasting tasting)
    {
        Tasting updTasting = await _tastingsRepository.UpdateAsync(id, tasting.Date, tasting.Theme);
        Dto.Tasting dtoUpdTasting = updTasting.ToDto();

        return Ok(dtoUpdTasting);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _tastingsRepository.DeleteAsync(id);
        return Ok();
    }
}
