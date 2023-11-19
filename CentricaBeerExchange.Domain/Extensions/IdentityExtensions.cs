using System.Diagnostics.CodeAnalysis;

namespace System.Security.Claims;

public static class IdentityExtensions
{
    public static bool TryGetClaimsIdentity(this ClaimsPrincipal claimsPrincipal, [NotNullWhen(true)] out ClaimsIdentity? claimsIdentity)
    {
        claimsIdentity = claimsPrincipal?.Identity as ClaimsIdentity;
        return claimsIdentity is not null;
    }

    public static bool TryGetClaimValue(this ClaimsIdentity claimsIdentity, string type, [NotNullWhen(true)] out string? value)
    {
        Claim? claim = claimsIdentity?.Claims?.FirstOrDefault(c => string.Equals(c.Type, type, StringComparison.OrdinalIgnoreCase));
        value = claim?.Value;
        return value is not null;
    }

    public static bool TryGetTokenId(this ClaimsIdentity claimsIdentity, [NotNullWhen(true)] out Guid tokenId)
    {
        tokenId = default;

        if (!TryGetClaimValue(claimsIdentity, AuthClaims.TokenId, out string? strTokenId))
            return false;

        return Guid.TryParse(strTokenId, out tokenId);
    }

    public static bool TryGetUserId(this ClaimsIdentity claimsIdentity, [NotNullWhen(true)] out int userId)
    {
        userId = default;

        if (!TryGetClaimValue(claimsIdentity, AuthClaims.UserId, out string? strUserId))
            return false;

        return int.TryParse(strUserId, out userId);
    }
}
