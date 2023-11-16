namespace CentricaBeerExchange.Domain.Models.Auth;

public class User
{
    public User(int id, string email, ERole role, string name, string department)
    {
        Id = id;
        Email = email;
        Role = role;
        Name = name ?? email;
        Department = department;
    }

    public int Id { get; }
    public string Email { get; }
    public ERole Role { get; }
    public string? Name { get; }
    public string? Department { get; }
}
