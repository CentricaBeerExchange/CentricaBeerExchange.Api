namespace CentricaBeerExchange.Domain.Helpers;

public static class CryptoHelper
{
    public static string GetHash(int value)
        => GetHash(value.ToString());

    public static string GetHash(string value)
        => BCrypt.Net.BCrypt.EnhancedHashPassword(value);

    public static bool IsValidHash(int value, string hash)
        => IsValidHash(value.ToString(), hash);

    public static bool IsValidHash(string value, string hash)
        => BCrypt.Net.BCrypt.EnhancedVerify(value, hash);
}
