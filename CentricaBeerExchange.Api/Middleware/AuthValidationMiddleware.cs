
namespace CentricaBeerExchange.Api.Middleware;

public class AuthValidationMiddleware
{
    private readonly ITimeProvider _timeProvider;
    private readonly IAuthRepository _authRepository;
    private readonly RequestDelegate _next;

    public AuthValidationMiddleware(ITimeProvider timeProvider, IAuthRepository authRepository, RequestDelegate next)
    {
        _timeProvider = timeProvider;
        _authRepository = authRepository;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (RequiresAuthorization(context) && await IsUnauthorizedAsync(context))
        {
            await HandleUnauthorizedAsync(context);
        }
        else
        {
            await _next.Invoke(context);
        }
    }

    private static bool RequiresAuthorization(HttpContext context)
    {
        Endpoint? endpoint = context.GetEndpoint();
        EndpointMetadataCollection? metadate = endpoint?.Metadata;
        AuthorizeAttribute? authorize = metadate?.GetMetadata<AuthorizeAttribute>();
        return authorize is not null;
    }

    private async Task<bool> IsUnauthorizedAsync(HttpContext context)
    {
        if (!context.User.TryGetClaimsIdentity(out ClaimsIdentity? claimsIdentity))
            return true;
        
        if (!claimsIdentity.IsAuthenticated)
            return true;
        
        if (!claimsIdentity.TryGetTokenId(out Guid tokenId))
            return true;

        TokenDetails? tokenDetails = await _authRepository.GetTokenAsync(tokenId);

        if (tokenDetails is null)
            return true;

        if (tokenDetails.TokenExpiryUtc < _timeProvider.UtcNow)
            return true;

        return false;
    }

    private static Task HandleUnauthorizedAsync(HttpContext context)
    {
        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;

    }
}
