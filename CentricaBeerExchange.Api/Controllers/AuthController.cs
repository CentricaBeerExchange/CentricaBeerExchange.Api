namespace CentricaBeerExchange.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email was not provided!");

        await _authService.LoginAsync(email);

        return Ok();
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetTokenAsync([FromBody] DtoAuth.TokenRequest? request)
    {
        if (request is null)
            return BadRequest("Token Request was not provided!");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email was not provided!");

        if (!request.VerificationCode.HasValue)
            return BadRequest("Verification Code was not provided!");

        TokenGenerationResult result = await _authService.GenerateTokenAsync(request.Email, request.VerificationCode.Value);

        return ProcessResult(result);
    }

    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] string? refreshToken)
    {
        if (!User.TryGetClaimsIdentity(out ClaimsIdentity? identity))
            return Unauthorized();

        if (!identity.IsAuthenticated)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest("Refresh Token was not provided!");

        TokenGenerationResult result = await _authService.RefreshTokenAsync(identity, refreshToken);

        return ProcessResult(result);
    }

    [Authorize]
    [HttpDelete("token/revoke")]
    public async Task<IActionResult> RevokeTokenAsync()
    {
        if (!User.TryGetClaimsIdentity(out ClaimsIdentity? identity))
            return Unauthorized();

        if (!identity.IsAuthenticated)
            return Unauthorized();

        bool wasRevoked = await _authService.RevokeTokenAsync(identity);

        if (wasRevoked)
            return Ok();

        return StatusCode(StatusCodes.Status304NotModified);
    }

    private IActionResult ProcessResult(TokenGenerationResult result)
    {
        if (result.Successful)
            return Ok(Map(result));

        if (result.IsUnauthorized)
            return Unauthorized(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }

    private DtoAuth.TokenResponse Map(TokenGenerationResult result)
        => new(Map(result.AccessToken!), Map(result.RefreshToken!));

    private DtoAuth.AccessToken Map(AccessToken token)
        => new(token.Token, token.ExpiresAtUtc);
}
