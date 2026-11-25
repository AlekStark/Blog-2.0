using System;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Utils;
public static class PasswordHasher
{
    // Генерация соли и хеша пароля
    public static (string Salt, string Hash) HashPassword(string password, int iterations = 100_000)
    {
        // Генерируем случайную соль (16 байт)
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        string saltHex = BitConverter.ToString(saltBytes).Replace("-", "").ToLowerInvariant();

        // Создаём хеш с помощью PBKDF2-HMAC-SHA256
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
        {
            byte[] hashBytes = pbkdf2.GetBytes(32); // 256 бит
            string hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            // Формат для хранения: алгоритм$итерации$соль$хеш
            string storedHash = $"pbkdf2_sha256${iterations}${saltHex}${hashHex}";
            return (saltHex, storedHash);
        }
    }

    // Проверка пароля
    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2_sha256") return false;

        int iterations = int.Parse(parts[1]);
        byte[] saltBytes = Enumerable.Range(0, parts[2].Length / 2)
            .Select(x => Convert.ToByte(parts[2].Substring(x * 2, 2), 16))
            .ToArray();
        byte[] expectedHashBytes = Enumerable.Range(0, parts[3].Length / 2)
            .Select(x => Convert.ToByte(parts[3].Substring(x * 2, 2), 16))
            .ToArray();

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
        {
            byte[] hashBytes = pbkdf2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(hashBytes, expectedHashBytes);
        }
    }
}
