namespace CentricaBeerExchange.Dtos.Auth;

[DataContract]
public class AccessToken
{
    public AccessToken()
        : this(string.Empty, string.Empty, default) { }

    public AccessToken(string email, string token, DateTime expiresAtUtc)
    {
        Email = email;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
    }

    [DataMember(Order = 1)] public string Email { get; set; }
    [DataMember(Order = 2)] public string Token { get; set; }
    [DataMember(Order = 3)] public DateTime ExpiresAtUtc { get; set; }
}
