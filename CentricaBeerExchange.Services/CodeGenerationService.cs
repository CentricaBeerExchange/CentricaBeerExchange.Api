using System.Security.Cryptography;

namespace CentricaBeerExchange.Services;

public class CodeGenerationService : ICodeGenerationService
{
    public int GenerateCode()
        => RandomNumberGenerator.GetInt32(100000, 999999);
}
