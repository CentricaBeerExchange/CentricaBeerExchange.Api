namespace CentricaBeerExchange.Dtos;

[DataContract]
public class Profile
{
    public Profile() { }

    public Profile(int userId, string email, string? name, string? department, string? thumbnail)
    {
        UserId = userId;
        Email = email;
        Name = name;
        Department = department;
        Thumbnail = thumbnail;
    }

    [DataMember(Order = 1)] public int UserId { get; set; }
    [DataMember(Order = 2)] public string? Email { get; set; }
    [DataMember(Order = 3)] public string? Name { get; set; }
    [DataMember(Order = 4)] public string? Department { get; set; }
    [DataMember(Order = 5)] public string? Thumbnail { get; set; }
}
