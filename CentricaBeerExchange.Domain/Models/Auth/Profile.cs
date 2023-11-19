namespace CentricaBeerExchange.Domain.Models.Auth;

public class Profile
{
    public Profile(int id, string email, string name, string department)
    {
        UserId = id;
        Email = email;
        Name = name;
        Department = department;
    }

    public int UserId { get; }
    public string Email { get; }
    public string Name { get; }
    public string Department { get; }
}
