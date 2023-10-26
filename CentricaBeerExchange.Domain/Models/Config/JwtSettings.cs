namespace CentricaBeerExchange.Domain.Models.Config;

public class JwtSettings
{
    public JwtSettings()
        : this(string.Empty, string.Empty, string.Empty, TimeSpan.Zero) { }

    public JwtSettings(string issuer, string audience, string key, TimeSpan expiryTime)
    {
        Issuer = issuer;
        Audience = audience;
        Key = key;
        ExpiryTime = expiryTime;
    }

    public string Issuer { get; private set; }
    public string Audience { get; private set; }
    public string Key { get; private set; }
    public TimeSpan ExpiryTime { get; private set; }

    public byte[] KeyBytes
        => Encoding.UTF8.GetBytes(Key);
}
