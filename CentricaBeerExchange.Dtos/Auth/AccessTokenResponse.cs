namespace CentricaBeerExchange.Dtos.Auth;

[DataContract]
public class TokenResponse
{
    public TokenResponse()
        : this(string.Empty, AccessToken.Empty, AccessToken.Empty) { }

    public TokenResponse(string email, AccessToken accessToken, AccessToken refreshToken)
    {
        Email = email;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    [DataMember(Order = 1)] public string Email { get; set; }
    [DataMember(Order = 2)] public AccessToken AccessToken { get; set; }
    [DataMember(Order = 3)] public AccessToken RefreshToken { get; set; }
}
