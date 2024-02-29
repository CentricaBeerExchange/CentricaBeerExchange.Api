namespace CentricaBeerExchange.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileRepository _profileRepository;

    public ProfileController(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    [HttpGet("me")]
    [ProducesResponseType<Dto.Profile>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOwnAsync()
    {
        if (!User.TryGetUserId(out int userId))
            return Forbid();

        Profile? profile = await _profileRepository.GetAsync(userId);

        if (profile is null)
            return NotFound();

        Dto.Profile dtoProfile = Map(profile);

        return Ok(dtoProfile);
    }

    [HttpPut("me")]
    [ProducesResponseType<Dto.Profile>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutOwnAsync([FromBody] Dto.Profile updatedProfile)
    {
        if (!User.TryGetUserId(out int userId))
            return Forbid();

        if (updatedProfile is null)
            return BadRequest("Provided Profile was empty!");

        Profile toUpdate = MapUpdate(updatedProfile);
        Profile? profile = await _profileRepository.UpdateAsync(userId, toUpdate);

        if (profile is null)
            return NotFound();

        Dto.Profile dtoProfile = Map(profile);

        return Ok(dtoProfile);
    }

    [HttpGet("")]
    [ProducesResponseType<Dto.Profile[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        Profile[] profiles = await _profileRepository.GetAsync();
        Dto.Profile[] dtoProfiles = Map(profiles);
        return Ok(dtoProfiles);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType<Dto.Profile>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(int userId)
    {
        Profile? profile = await _profileRepository.GetAsync(userId);

        if (profile is null)
            return NotFound();

        Dto.Profile dtoProfile = Map(profile);
        return Ok(dtoProfile);
    }

    private static Dto.Profile[] Map(IEnumerable<Profile> profiles)
        => profiles?.Select(Map).ToArray() ?? [];

    private static Dto.Profile Map(Profile profile)
        => new(
            userId: profile.Id,
            email: profile.Email,
            name: profile.Name,
            department: profile.Department,
            thumbnail: profile.Thumbnail
        );

    private static Profile MapUpdate(Dto.Profile dtoProfile)
        => new(
            Id: 0,
            Email: string.Empty,
            Name: dtoProfile.Name,
            Department: dtoProfile.Department,
            Thumbnail: dtoProfile.Thumbnail
        );
}
