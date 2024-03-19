using Azure.Communication.Email;
using System.Diagnostics;

namespace CentricaBeerExchange.Services;

public class EmailService : IEmailService
{
    private readonly string _senderAddress;
    private readonly EmailClient _client;

    const string MESSAGE_PREFIX = "Hi, Here is your one-time login code for Centrica Beer Exchange:";
    const string MESSAGE_POSTFIX = "Code will expire in 15 minutes!";
    const string PLACEHOLDER_CODE = "{CODE}";
    const string HTML_TEMPLATE = $@"
<html>
<body>
    <p>{MESSAGE_PREFIX}</p>
    <h2>{PLACEHOLDER_CODE}</h2>
    <br />
    <em>{MESSAGE_POSTFIX}</em>
</body>
</html>
";

    public EmailService(EmailSettings emailSettings)
    {
        _senderAddress = emailSettings.SenderAddress;
        _client = new EmailClient(emailSettings.ConnectionString);
    }

    public async Task SendCodeAsync(string email, int code)
    {
        Debug.WriteLine(
            $"::::VERIFICATION::::\r\n" +
            $"\tEmail: {email}\r\n" +
            $"\tCode: {code}\r\n" +
            $"\tHash: {CryptoHelper.GetHash(code)}"
        );

        if (Debugger.IsAttached)
            return;

        (string html, string plainText) = GetMessage(code);
        EmailSendOperation sendOperation = await _client.SendAsync(
            wait: Azure.WaitUntil.Completed,
            senderAddress: _senderAddress,
            recipientAddress: email,
            subject: "Verification code for Centrica Beer Exchange",
            htmlContent: html,
            plainText
        );
    }

    public static (string html, string plainText) GetMessage(int code)
    {

        string html = HTML_TEMPLATE.Replace(PLACEHOLDER_CODE, $"{code}");
        string plainText = $"{MESSAGE_PREFIX}\r\n\r\n{code}\r\n\r\n{MESSAGE_POSTFIX}";
        return (html, plainText);
    }
}
