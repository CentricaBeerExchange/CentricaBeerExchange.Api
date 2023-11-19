namespace CentricaBeerExchange.Dtos.Auth;

[DataContract]
public class TokenResponse
{
    public TokenResponse()
        : this(AccessToken.Empty, AccessToken.Empty) { }

    public TokenResponse(AccessToken accessToken, AccessToken refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    [DataMember(Order = 1)] public AccessToken AccessToken { get; set; }
    [DataMember(Order = 2)] public AccessToken RefreshToken { get; set; }
}
