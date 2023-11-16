namespace CentricaBeerExchange.Services;

public class EmailService : IEmailService
{
    public async Task SendCodeAsync(string email, int code)
    {
        await Task.CompletedTask;
        System.Diagnostics.Debug.WriteLine(
            $"::::VERIFICATION::::\r\n" +
            $"\tEmail: {email}\r\n" +
            $"\tCode: {code}\r\n" +
            $"\tHash: {CryptoHelper.GetHash(code)}");
    }
}
