using System.Security.Claims;

namespace CentricaBeerExchange.Services;

public class AuthService : IAuthService
{
    private readonly ITimeProvider _timeProvider;
    private readonly ICodeGenerationService _codeGenerationService;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly IAuthRepository _authRepository;

    public AuthService(ITimeProvider timeProvider, ICodeGenerationService codeGenerationService, IEmailService emailService, ITokenService tokenService, IAuthRepository authRepository)
    {
        _timeProvider = timeProvider;
        _codeGenerationService = codeGenerationService;
        _emailService = emailService;
        _tokenService = tokenService;
        _authRepository = authRepository;
    }

    public async Task LoginAsync(string email)
    {
        int code = _codeGenerationService.GenerateCode();

        await _authRepository.UpsertVerificationCodeAsync(email, code);
        await _emailService.SendCodeAsync(email, code);
    }

    public async Task<TokenGenerationResult> GenerateTokenAsync(string email, int verificationCode)
    {
        Verification? verification = await _authRepository.GetVerificationAsync(email);

        if (verification is null)
            return new TokenGenerationResult($"Could not verify code for '{email}'!", true);

        if (verification.ValidUntilUtc < _timeProvider.UtcNow)
        {
            await _authRepository.RemoveVerificationAsync(email);
            return new TokenGenerationResult($"Invalid verification code for '{email}'!", true);
        }

        if (!CryptoHelper.IsValidHash(verificationCode, verification.CodeHash))
            return new TokenGenerationResult($"Invalid verification code for '{email}'!", true);

        try
        {
            User user = await _authRepository.GetOrCreateUserAsync(email);

            TokenGenerationResult tokenResult = _tokenService.Generate(user);

            await _authRepository.RemoveVerificationAsync(email);
            await _authRepository.UpsertTokenAsync(tokenResult.AsTokenDetails(user.Id));

            return tokenResult;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<TokenGenerationResult> RefreshTokenAsync(ClaimsIdentity identity, string refreshToken)
    {
        if (!identity.TryGetTokenId(out Guid tokenId))
            return new TokenGenerationResult($"Failed to get Token Id from authorized context!", true);

        if (!identity.TryGetUserId(out int userId))
            return new TokenGenerationResult($"Failed to get User Id from authorized context!", true);

        TokenDetails? tokenDetails = await _authRepository.GetTokenAsync(tokenId);

        if (tokenDetails is null)
            return new TokenGenerationResult($"Invalid Access Token!", true);

        if (!string.Equals(tokenDetails.RefreshToken, refreshToken, StringComparison.OrdinalIgnoreCase))
            return new TokenGenerationResult($"Invalid Refresh Token!", true);

        User user = await _authRepository.GetUserAsync(userId);

        if (tokenDetails.RefreshExpiryUtc < _timeProvider.UtcNow)
        {
            await _authRepository.RemoveTokenAsync(user.Id);
            return new TokenGenerationResult($"Refresh Token has expired!", true);
        }

        TokenGenerationResult tokenResult = _tokenService.Generate(user);

        await _authRepository.UpsertTokenAsync(tokenResult.AsTokenDetails(user.Id));

        return tokenResult;
    }

    public async Task<bool> RevokeTokenAsync(ClaimsIdentity identity)
    {
        if (!identity.TryGetUserId(out int userId))
            return false;

        bool wasDeleted = await _authRepository.RemoveTokenAsync(userId);

        return wasDeleted;
    }
}
