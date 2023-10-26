namespace CentricaBeerExchange.Dtos.Auth;

[DataContract]
public class AccessToken
{
    public AccessToken()
        : this(string.Empty, string.Empty, default) { }

    public AccessToken(string clientId, string token, DateTime expiresAtUtc)
    {
        ClientId = clientId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
    }

    [DataMember(Order = 1)] public string ClientId { get; set; }
    [DataMember(Order = 2)] public string Token { get; set; }
    [DataMember(Order = 3)] public DateTime ExpiresAtUtc { get; set; }
}
