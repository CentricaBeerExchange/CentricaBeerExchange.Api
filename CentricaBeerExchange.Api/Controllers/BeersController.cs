namespace CentricaBeerExchange.Api.Controllers;

[ApiController]
[Route("api/breweries")]
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
        Dto.Beer[] dtoBeers = Map(beers);

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

        Dto.Beer dtoBeer = Map(beer);
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

        Dto.Beer dtoNewBeer = Map(newBeer);
        return Ok(dtoNewBeer);
    }

    [HttpPut("{id:int}")]
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

        Dto.Beer dtoUpdBeer = Map(updBeer);
        return Ok(dtoUpdBeer);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _beersRepository.DeleteAsync(id);
        return Ok();
    }

    private static Dto.Beer[] Map(IEnumerable<Beer> beers)
        => beers?.Select(Map).ToArray() ?? [];

    private static Dto.Beer Map(Beer beer)
        => new(
            id: beer.Id,
            name: beer.Name,
            brewery: Map(beer.Brewery),
            style: Map(beer.Style),
            rating: beer.Rating,
            abv: beer.ABV,
            untappdId: beer.UntappdId
        );

    private static Dto.BreweryMeta Map(BreweryMeta brewery)
        => new(
            id: brewery.Id,
            name: brewery.Name
        );

    private static Dto.Style Map(Style style)
        => new(
            id: style.Id,
            name: style.Name
        );
}
