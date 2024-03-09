namespace CentricaBeerExchange.Api.Controllers;

[ApiController]
[Route("api")]
public class DefaultController : ControllerBase
{
    [HttpGet("HelloWorld")]
    public IActionResult Hello()
        => Ok(new { Message = "Hello World!" });

    [Authorize]
    [HttpGet("HelloAuthenticated")]
    public IActionResult HelloAuth()
        => Ok(new { Message = $"Hello {User.Identity?.Name} :D" });

    [Authorize]
    [MinimumRole(ERole.Admin)]
    [HttpGet("HelloAuthenticated/MinimumRole")]
    public IActionResult HelloAuthWithMinRole()
        => Ok(new { Message = $"Hello {ERole.Admin} {User.Identity?.Name} :D" });
}
