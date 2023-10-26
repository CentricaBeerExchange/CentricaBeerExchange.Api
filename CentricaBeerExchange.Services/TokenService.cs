using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CentricaBeerExchange.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly AuthSecrets _authSecrets;

    public TokenService(JwtSettings jwtSettings, AuthSecrets authSecrets)
    {
        _jwtSettings = jwtSettings;
        _authSecrets = authSecrets;
    }

    public async Task<TokenGenerationResult> GetAsync(string clientId, string clientSecret)
    {
        await Task.CompletedTask;
        // TODO: Move secrets from config to Db?

        if (!_authSecrets.TryGet(clientId, out string? actualSecret, out string[]? roles))
            return new TokenGenerationResult("Invalid Client Id!", false);

        if (!string.Equals(clientSecret, actualSecret))
            return new TokenGenerationResult("Invalid Client Secret!", true);

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, clientId)
        };

        foreach (string r in roles)
            claims.Add(new Claim(ClaimTypes.Role, r));

        (string accessToken, DateTime expiresAtUtc) = GenerateToken(claims);

        return new TokenGenerationResult(accessToken, expiresAtUtc);
    }

    private (string accessToken, DateTime expiresAtUtc) GenerateToken(IEnumerable<Claim> claims)
    {
        SymmetricSecurityKey authSigningKey = new(_jwtSettings.KeyBytes);
        SigningCredentials signingCredentials = new(authSigningKey, SecurityAlgorithms.HmacSha256);

        DateTime expiresAtUtc = DateTime.UtcNow
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
