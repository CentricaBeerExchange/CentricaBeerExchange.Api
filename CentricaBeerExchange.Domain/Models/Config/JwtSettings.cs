namespace CentricaBeerExchange.Domain.Models.Config;

public class JwtSettings
{
    public JwtSettings()
        : this(string.Empty, string.Empty, string.Empty, default, default) { }

    public JwtSettings(string issuer, string audience, string key, int tokenExpiryHours, int refreshExpiryDays)
    {
        Issuer = issuer;
        Audience = audience;
        Key = key;
        TokenExpiryHours = tokenExpiryHours;
        RefreshExpiryDays = refreshExpiryDays;
    }

    public string Issuer { get; private set; }
    public string Audience { get; private set; }
    public string Key { get; private set; }
    public int TokenExpiryHours { get; private set; }
    public int RefreshExpiryDays { get; private set; }

    public byte[] KeyBytes
        => Encoding.UTF8.GetBytes(Key);
}
