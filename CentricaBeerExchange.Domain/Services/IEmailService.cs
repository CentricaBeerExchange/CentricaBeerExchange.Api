namespace CentricaBeerExchange.Domain.Services;

public interface IEmailService
{
    Task SendCodeAsync(string email, int code);
}
