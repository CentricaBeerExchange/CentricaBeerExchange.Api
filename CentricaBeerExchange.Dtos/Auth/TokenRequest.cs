namespace CentricaBeerExchange.Dtos.Auth;

[DataContract]
public class TokenRequest
{
    public TokenRequest()
    {

    }
    public TokenRequest(string email, int verificationCode)
    {
        Email = email;
        VerificationCode = verificationCode;
    }

    [DataMember(Order = 1)] public string? Email { get; set; }
    [DataMember(Order = 2)] public int? VerificationCode { get; set; }
}
