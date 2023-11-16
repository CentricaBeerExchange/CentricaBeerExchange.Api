
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

    public async Task<TokenGenerationResult> GetTokenAsync(string email, int verificationCode)
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
            return tokenResult;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
