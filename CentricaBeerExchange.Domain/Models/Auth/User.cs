namespace CentricaBeerExchange.Domain.Models.Auth;

public class User
{
    public User(int id, string email, ERole role)
    {
        Id = id;
        Email = email;
        Role = role;
    }

    public int Id { get; }
    public string Email { get; }
    public ERole Role { get; }
}
