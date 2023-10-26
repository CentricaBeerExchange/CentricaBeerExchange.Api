namespace CentricaBeerExchange.Api.Controllers.Auth;

[ApiController]
[Route("api/auth/token")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> GetTokenAsync([FromBody] DtoAuth.TokenRequest? request)
    {
        if (request is null)
            return BadRequest("Token Request was not provided!");

        if (string.IsNullOrWhiteSpace(request.ClientId))
            return BadRequest("Client Id was not provided!");

        if (string.IsNullOrWhiteSpace(request.ClientSecret))
            return BadRequest("Client Secret was not provided!");

        TokenGenerationResult result = await _tokenService.GetAsync(request.ClientId, request.ClientSecret);

        if (result.Successful)
            return Ok(new DtoAuth.AccessToken(request.ClientId, result.AccessToken, result.ExpiresAtUtc));

        if (result.IsUnauthorized)
            return Unauthorized(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }
}
