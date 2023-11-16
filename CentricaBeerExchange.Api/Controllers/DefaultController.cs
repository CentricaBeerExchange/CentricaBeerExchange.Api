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
        => Ok(new { Message = $"Hello {User.Identity?.Name} User :D" });
}
