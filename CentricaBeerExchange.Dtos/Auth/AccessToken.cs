namespace CentricaBeerExchange.Dtos.Auth;

[DataContract]
public class AccessToken
{
    public AccessToken()
    {
        Token = string.Empty;
    }

    public AccessToken(string token, DateTime expiresAtUtc)
    {
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
    }

    [DataMember(Order = 1)] public string Token { get; set; }
    [DataMember(Order = 2)] public DateTime ExpiresAtUtc { get; set; }

    internal static AccessToken Empty => new();
}
