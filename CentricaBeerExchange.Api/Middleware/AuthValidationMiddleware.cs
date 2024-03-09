namespace CentricaBeerExchange.Api.Middleware;

public class AuthValidationMiddleware
{
    private static readonly char[] CommaDelimiter = [','];

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
        if (await ShouldInvokeNextAsync(context))
            await _next.Invoke(context);
    }

    private async Task<bool> ShouldInvokeNextAsync(HttpContext context)
    {
        if (!RequiresAuthorization(context, out string[] requiredRoles))
            return true;

        (bool isAuthorized, int statusCode, string errorMessage) = await IsAuthorizedAsync(context, requiredRoles);

        if (isAuthorized)
            return true;

        await HandleUnauthorizedAsync(context, statusCode, errorMessage);
        return false;
    }

    private static bool RequiresAuthorization(HttpContext context, out string[] requiredRoles)
    {
        Endpoint? endpoint = context.GetEndpoint();
        EndpointMetadataCollection? metadate = endpoint?.Metadata;
        AuthorizeAttribute? authorize = metadate?.GetMetadata<AuthorizeAttribute>();
        requiredRoles = ParseRoles(authorize);
        return authorize is not null;
    }

    private static string[] ParseRoles(AuthorizeAttribute? authorize)
    {
        if (authorize is null)
            return [];

        if (string.IsNullOrWhiteSpace(authorize.Roles))
            return [];

        return authorize.Roles.Split(CommaDelimiter, StringSplitOptions.RemoveEmptyEntries);
    }

    private async Task<(bool isAuthorized, int statusCode, string errorMessage)> IsAuthorizedAsync(HttpContext context, string[] requiredRoles)
    {
        if (!context.User.TryGetClaimsIdentity(out ClaimsIdentity? claimsIdentity))
            return (false, StatusCodes.Status401Unauthorized, "Failed to get Identity!");

        if (!claimsIdentity.IsAuthenticated)
            return (false, StatusCodes.Status401Unauthorized, "User is NOT authenticated!");

        if (!claimsIdentity.TryGetTokenId(out Guid tokenId))
            return (false, StatusCodes.Status401Unauthorized, "Claims did NOT contain User Id!");

        TokenDetails? tokenDetails = await _authRepository.GetTokenAsync(tokenId);

        if (tokenDetails is null)
            return (false, StatusCodes.Status401Unauthorized, "Failed to get token details!");

        if (tokenDetails.TokenExpiryUtc < _timeProvider.UtcNow)
            return (false, StatusCodes.Status401Unauthorized, "Auth Token as expired!");

        if (MissingRequiredRole(claimsIdentity, requiredRoles, out string roleErrorMessage))
            return (false, StatusCodes.Status401Unauthorized, roleErrorMessage);

        return (true, StatusCodes.Status200OK, string.Empty);
    }

    private static bool MissingRequiredRole(ClaimsIdentity claimsIdentity, string[] requiredRoles, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (requiredRoles.Length == 0)
            return false;

        if (!claimsIdentity.TryGetClaimValue(ClaimTypes.Role, out string? role))
        {
            errorMessage = "Claims dod NOT contain Role!";
            return true;
        }

        if (!requiredRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            errorMessage = "Missing required role!";
            return true;
        }

        return false;
    }

    private static Task HandleUnauthorizedAsync(HttpContext context, int statusCode, string errorMessage)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(errorMessage);
    }
}
