namespace CentricaBeerExchange.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("/login")]
    public async Task<IActionResult> LoginAsync([FromBody] string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email was not provided!");

        await _authService.LoginAsync(email);

        return Ok();
    }

    [HttpPost("/token")]
    public async Task<IActionResult> GetTokenAsync([FromBody] DtoAuth.TokenRequest? request)
    {
        if (request is null)
            return BadRequest("Token Request was not provided!");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email was not provided!");

        if (!request.VerificationCode.HasValue)
            return BadRequest("Verification Code was not provided!");

        TokenGenerationResult result = await _authService.GetTokenAsync(request.Email, request.VerificationCode.Value);

        if (result.Successful)
            return Ok(new DtoAuth.AccessToken(request.Email, result.AccessToken, result.ExpiresAtUtc));

        if (result.IsUnauthorized)
            return Unauthorized(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }
}
