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
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        ];

        (string accessToken, DateTime expiresAtUtc) = GenerateToken(claims);

        return new TokenGenerationResult(accessToken, expiresAtUtc);
    }

    private (string accessToken, DateTime expiresAtUtc) GenerateToken(IEnumerable<Claim> claims)
    {
        SymmetricSecurityKey authSigningKey = new(_jwtSettings.KeyBytes);
        SigningCredentials signingCredentials = new(authSigningKey, SecurityAlgorithms.HmacSha256);

        DateTime expiresAtUtc = _timeProvider.UtcNow
            .Add(_jwtSettings.ExpiryTime)
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

        return (tokenHandler.WriteToken(secToken), expiresAtUtc);
    }
}
