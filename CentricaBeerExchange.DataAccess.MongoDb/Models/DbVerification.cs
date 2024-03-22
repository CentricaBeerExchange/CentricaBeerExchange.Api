namespace CentricaBeerExchange.DataAccess.MongoDb.Models;

//internal record DbVerification(Guid Id, string Email, string CodeHash, DateTime CreatedAtUtc, DateTime ValidUntilUtc)
//    : Verification(Email, CodeHash, ValidUntilUtc);

internal class DbVerification(Guid id, string email, string codeHash, DateTime createdAtUtc, DateTime validUntilUtc)
{
    public Guid Id { get; } = id;
    public string Email { get; } = email;
    public string CodeHash { get; } = codeHash;
    public DateTime CreatedAtUtc { get; } = createdAtUtc;
    public DateTime ValidUntilUtc { get; } = validUntilUtc;

    public Verification Map()
        => new(Email, CodeHash, ValidUntilUtc);
}
