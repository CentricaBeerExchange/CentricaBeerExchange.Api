namespace CentricaBeerExchange.Api.Controllers;

[ApiController]
[Route("api/styles")]
public class StylesController : ControllerBase
{
    private static readonly char[] _listSeparator = [','];
    private static readonly StringSplitOptions _listSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    private readonly IStylesRepository _stylesRepository;

    public StylesController(IStylesRepository stylesRepository)
    {
        _stylesRepository = stylesRepository;
    }

    [HttpGet("")]
    [ProducesResponseType<Dto.Style[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        Style[] styles = await _stylesRepository.GetAsync();
        Dto.Style[] dtoStyles = styles.Select(Map).ToArray();

        return Ok(dtoStyles);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<Dto.Style>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(short id)
    {
        Style? style = await _stylesRepository.GetAsync(id);

        if (style is null)
            return NotFound($"Style with Id '{id}' was NOT found!");

        Dto.Style dtoStyle = Map(style);
        return Ok(dtoStyle);
    }

    [HttpPost("")]
    [ProducesResponseType<Dto.Style>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] Dto.Style style)
    {
        Style domainStyle = Map(style);
        Style updStyle = await _stylesRepository.UpsertAsync(domainStyle);

        Dto.Style dtoUpdStyle = Map(updStyle);
        return Ok(dtoUpdStyle);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostAsync([FromBody] Dto.Style[] styles)
    {
        Style[] domainStyles = styles.Select(Map).ToArray();
        Style[] updStyles = await _stylesRepository.UpsertAsync(domainStyles);

        Dto.Style[] dtoUpdStyle = updStyles.Select(Map).ToArray();
        return Ok(dtoUpdStyle);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromRoute] short id)
    {
        await _stylesRepository.DeleteAsync(id);
        return Ok();
    }

    [HttpDelete("bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromQuery] string ids)
    {
        short[] parsedIds = ids.Split(_listSeparator, _listSplitOptions).Select(short.Parse).ToArray();
        await _stylesRepository.DeleteAsync(parsedIds);
        return Ok();
    }

    private static Dto.Style Map(Style style)
        => new(style.Id, style.Name);

    private static Style Map(Dto.Style dtoStyle)
        => new(dtoStyle.Id, dtoStyle.Name);
}
