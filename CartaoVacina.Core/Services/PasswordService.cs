using System.Security.Cryptography;
using System.Text;

namespace CartaoVacina.Core.Services;

public class PasswordService : IPasswordService
{
    public (string hash, string salt) HashPassword(string password)
    {
        var salt = GenerateSalt();
        var hash = HashPasswordWithSalt(password, salt);
        return (hash, salt);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var computedHash = HashPasswordWithSalt(password, salt);
        return computedHash == hash;
    }

    private static string GenerateSalt()
    {
        var salt = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }

    private static string HashPasswordWithSalt(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hashBytes);
    }
} 