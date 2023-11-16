namespace CentricaBeerExchange.Domain.Services;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}
