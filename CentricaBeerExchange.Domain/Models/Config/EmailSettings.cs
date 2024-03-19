namespace CentricaBeerExchange.Domain.Models.Config;

public class EmailSettings
{
    public EmailSettings(string senderAddress)
    {
        SenderAddress = senderAddress;
        ConnectionString = string.Empty;
    }

    public string SenderAddress { get; private set; }
    public string ConnectionString { get; private set; }

    public void AttachConnectionString(string connectionString)
        => ConnectionString = connectionString;
}
