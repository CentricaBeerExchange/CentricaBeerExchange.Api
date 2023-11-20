namespace CentricaBeerExchange.Domain.Models;

public class Profile
{
    public Profile(int id, string email, string? name, string? department, string? thumbnail)
    {
        UserId = id;
        Email = email;
        Name = name;
        Department = department;
        Thumbnail = thumbnail;
    }

    public int UserId { get; }
    public string Email { get; }
    public string? Name { get; }
    public string? Department { get; }
    public string? Thumbnail { get; }
}
