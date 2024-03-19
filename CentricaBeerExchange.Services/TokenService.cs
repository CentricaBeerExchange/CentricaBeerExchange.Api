using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CentricaBeerExchange.Services;

public class TokenService : ITokenService
{
    private readonly ITimeProvider _timeProvider;
    private readonly JwtSettings _jwtSettings;

    public TokenService(ITimeProvider timeProvider, JwtSettings jwtSettings)
    {
        _timeProvider = timeProvider;
        _jwtSettings = jwtSettings;
    }

    public TokenGenerationResult Generate(User user)
    {
        Guid tokenId = Guid.NewGuid();

        List<Claim> claims =
        [
            new Claim(AuthClaims.TokenId, tokenId.ToString()),
            new Claim(AuthClaims.UserId, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        ];

        AccessToken accessToken = GenerateToken(claims);
        AccessToken refreshToken = GenerateRefreshToken();

        return new TokenGenerationResult(tokenId, accessToken, refreshToken);
    }

    public bool TryGetIdFromExpiredToken(string accessToken, out Guid tokenId, [NotNullWhen(false)] out string? errorMessage)
    {
        (SymmetricSecurityKey authSigningKey, SigningCredentials signingCredentials) = GetJwtSigning();

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authSigningKey,
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        errorMessage = null;
        tokenId = Guid.Empty;

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            errorMessage = "Invalid Access Token";

        if (!principal.TryGetClaimsIdentity(out ClaimsIdentity? claimsIdentity))
            errorMessage = "Failed to get Identity from expired Access Token!";

        else if (!claimsIdentity.TryGetTokenId(out tokenId))
            errorMessage = "Failed to get Id from expired Access Token!";

        return string.IsNullOrWhiteSpace(errorMessage) && tokenId != Guid.Empty;
    }

    private AccessToken GenerateToken(IEnumerable<Claim> claims)
    {
        (SymmetricSecurityKey authSigningKey, SigningCredentials signingCredentials) = GetJwtSigning();

        DateTime expiresAtUtc = _timeProvider.UtcNow
            .AddHours(_jwtSettings.TokenExpiryHours)
            .AddMilliseconds(100);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = expiresAtUtc,
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(claims)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken secToken = tokenHandler.CreateToken(tokenDescriptor);

        return new AccessToken(tokenHandler.WriteToken(secToken), expiresAtUtc);
    }

    private (SymmetricSecurityKey authSigningKey, SigningCredentials signingCredentials) GetJwtSigning()
    {
        SymmetricSecurityKey authSigningKey = new(_jwtSettings.KeyBytes);
        SigningCredentials signingCredentials = new(authSigningKey, SecurityAlgorithms.HmacSha256);

        return (authSigningKey, signingCredentials);
    }

    private AccessToken GenerateRefreshToken()
    {
        string refreshToken = Guid.NewGuid().ToString();

        DateTime expiresAtUtc = _timeProvider.UtcNow
            .AddDays(_jwtSettings.RefreshExpiryDays)
            .AddMilliseconds(100);

        return new AccessToken(refreshToken, expiresAtUtc);
    }
}
