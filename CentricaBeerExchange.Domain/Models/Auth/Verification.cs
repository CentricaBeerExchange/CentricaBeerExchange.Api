namespace CentricaBeerExchange.Domain.Models.Auth;

public class Verification
{
    public Verification(string email, string codeHash, DateTime validUntilUtc)
    {
        Email = email;
        CodeHash = codeHash;
        ValidUntilUtc = validUntilUtc;
    }

    public string Email { get; }
    public string CodeHash { get; }
    public DateTime ValidUntilUtc { get; }
}
