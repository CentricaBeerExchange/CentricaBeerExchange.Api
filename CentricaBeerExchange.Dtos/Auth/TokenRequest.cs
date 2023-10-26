namespace CentricaBeerExchange.Dtos.Auth;

[DataContract]
public class TokenRequest
{
    public TokenRequest(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

    [DataMember(Order = 1)] public string? ClientId { get; set; }
    [DataMember(Order = 2)] public string? ClientSecret { get; set; }
}
