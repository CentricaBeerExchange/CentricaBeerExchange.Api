namespace CentricaBeerExchange.Api.Controllers;

[ApiController]
[Route("api/breweries")]
public class BreweriesController : ControllerBase
{
    private readonly IBreweriesRepository _breweriesRepository;

    public BreweriesController(IBreweriesRepository breweriesRepository)
    {
        _breweriesRepository = breweriesRepository;
    }

    [HttpGet("")]
    [ProducesResponseType<Dto.Brewery[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        Brewery[] breweries = await _breweriesRepository.GetAsync();
        Dto.Brewery[] dtoBreweries = breweries.Select(Map).ToArray();

        return Ok(dtoBreweries);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<Dto.Brewery[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromRoute] int id)
    {
        Brewery? brewery = await _breweriesRepository.GetAsync(id);

        if (brewery is null)
            return NotFound();

        Dto.Brewery dtoBrewery = Map(brewery);
        return Ok(dtoBrewery);
    }

    [HttpPost("")]
    [ProducesResponseType<Dto.Brewery>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] Dto.Brewery brewery)
    {
        Brewery newBrewery = await _breweriesRepository.AddAsync(
            brewery.Name,
            brewery.UntappdId,
            brewery.Location,
            brewery.Type,
            brewery.Thumbnail
        );

        Dto.Brewery dtoNewBrewery = Map(newBrewery);
        return Ok(dtoNewBrewery);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<Dto.Brewery>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromRoute] int id, [FromBody] Dto.Brewery brewery)
    {
        Brewery updBrewery = await _breweriesRepository.UpdateAsync(id,
            brewery.Name,
            brewery.UntappdId,
            brewery.Location,
            brewery.Type,
            brewery.Thumbnail
        );

        Dto.Brewery dtoUpdBrewery = Map(updBrewery);
        return Ok(dtoUpdBrewery);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _breweriesRepository.DeleteAsync(id);
        return Ok();
    }

    private static Dto.Brewery Map(Brewery brewery)
        => new(brewery.Id, brewery.Name, brewery.UntappdId, brewery.Location, brewery.Type, brewery.Thumbnail);
}
