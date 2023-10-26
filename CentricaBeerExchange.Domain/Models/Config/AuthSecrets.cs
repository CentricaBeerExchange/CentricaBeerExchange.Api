namespace CentricaBeerExchange.Domain.Models.Config;

public class AuthSecrets
{
    public AuthSecrets()
        : this(ClientSecret.Empty, ClientSecret.Empty) { }

    public AuthSecrets(ClientSecret readOnly, ClientSecret readWrite)
    {
        ReadOnly = readOnly;
        ReadWrite = readWrite;
    }

    public ClientSecret ReadOnly { get; }
    public ClientSecret ReadWrite { get; }

    public bool TryGet(string id, [NotNullWhen(true)] out string? secret, [NotNullWhen(true)] out string[]? roles)
    {
        secret = null;
        roles = null;

        if (string.IsNullOrWhiteSpace(id))
            return false;

        if (id.Equals(ReadOnly.Id))
        {
            secret = ReadOnly.Secret;
            roles = new[] { Auth.AuthRoles.READ };
        }

        else if (id.Equals(ReadWrite.Id))
        {
            secret = ReadWrite.Secret;
            roles = new[] { Auth.AuthRoles.READ, Auth.AuthRoles.WRITE };
        }

        return secret is not null && roles is not null;
    }
}
