namespace CentricaBeerExchange.DataAccess.MongoDb.Models;

internal class DbUser(Guid id, string email, ERole role, string name, DateTime createdAtUtc)
{
    public Guid Id { get; } = id;
    public string Email { get; } = email;
    public ERole Role { get; } = role;
    public string Name { get; } = name;
    public DateTime CreatedAtUtc { get; } = createdAtUtc;

    public User Map()
        => new(Id.ToInt(), Email, Role, Name ?? string.Empty);
}
