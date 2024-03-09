namespace CentricaBeerExchange.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/beers")]
public class BeersController : ControllerBase
{
    private readonly IBeersRepository _beersRepository;

    public BeersController(IBeersRepository beersRepository)
    {
        _beersRepository = beersRepository;
    }

    [HttpGet("")]
    [ProducesResponseType<Dto.Beer[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        Beer[] beers = await _beersRepository.GetAsync();
        Dto.Beer[] dtoBeers = beers.ToDto();

        return Ok(dtoBeers);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<Dto.Beer>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] int id)
    {
        Beer? beer = await _beersRepository.GetAsync(id);

        if (beer is null)
            return NotFound();

        Dto.Beer dtoBeer = beer.ToDto();
        return Ok(dtoBeer);
    }

    [HttpPost("")]
    [ProducesResponseType<Dto.Beer>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] Dto.Beer beer)
    {
        Beer newBeer = await _beersRepository.AddAsync(
            beer.Name,
            beer.BreweryId,
            beer.StyleId,
            beer.Rating,
            beer.ABV,
            beer.UntappdId
        );

        Dto.Beer dtoNewBeer = newBeer.ToDto();
        return Ok(dtoNewBeer);
    }

    [HttpPut("{id:int}")]
    [MinimumRole(ERole.Editor)]
    [ProducesResponseType<Dto.Beer>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Dto.Beer beer)
    {
        Beer updBeer = await _beersRepository.UpdateAsync(id,
            beer.Name,
            beer.BreweryId,
            beer.StyleId,
            beer.Rating,
            beer.ABV,
            beer.UntappdId
        );

        Dto.Beer dtoUpdBeer = updBeer.ToDto();
        return Ok(dtoUpdBeer);
    }

    [HttpDelete("{id:int}")]
    [MinimumRole(ERole.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _beersRepository.DeleteAsync(id);
        return Ok();
    }
}
