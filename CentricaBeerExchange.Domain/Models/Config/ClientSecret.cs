namespace CentricaBeerExchange.Domain.Models.Config;

public class ClientSecret
{
    internal static ClientSecret Empty => new();

    public ClientSecret()
        : this(string.Empty, string.Empty) { }

    public ClientSecret(string clientId, string clientSecret)
    {
        Id = clientId;
        Secret = clientSecret;
    }

    public string Id { get; private set; }
    public string Secret { get; private set; }
}
