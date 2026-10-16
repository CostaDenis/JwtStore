using System.Security.Cryptography;
using JwtStore.Core.SharedContext.ValueObjects;

namespace JwtStore.Core.AccountContext.ValueObjects;

public class Password : ValueObject
{
    private const string Valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private const string Special = "!@#$%ˆ&*(){}[];";

    public string Hash { get; } = string.Empty;
    public string ResetCode { get; }
        = Guid.NewGuid().ToString("N")[0..8].ToUpper();

    protected Password()
    {
    }

    public Password(string? password = null)
    {
        if (string.IsNullOrEmpty(password) ||
            string.IsNullOrWhiteSpace(password))
            password = Generate();

        Hash = Hashing(password);
    }

    private static string Generate(
        short lenght = 16, bool includeSpecialChars = true,
        bool upperCase = false
    )
    {
        var chars = includeSpecialChars ? (Valid + Special) : Valid;
        var startRandom = upperCase ? 26 : 0;
        var index = 0;
        var res = new char[lenght];
        var rdn = new Random();

        while (index < lenght)
            res[index++] = chars[rdn.Next(startRandom, chars.Length)];

        return new string(res);
    }

    private static string Hashing(
        string password, short saltSize = 16,
        short keySize = 32, int iterations = 10000,
        char splitChar = '.'
    )
    {
        if (string.IsNullOrEmpty(password))
            throw new Exception("A senha não pode ser nula ou espaço vazio.");

        //Cocatena a Password com uma chave definida no servidor
        password += Configuration.Secrets.PasswordSaltKey;

        //Gera um salt aleatório e criptograficamente seguro
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

        //Deriva a chave usando PBKDF2 + SHA-256
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize
        );

        var keyBase64 = Convert.ToBase64String(key);
        var saltBase64 = Convert.ToBase64String(salt);

        return $"{iterations}{splitChar}{salt}{splitChar}{key}";
    }

    private static bool Verify(
    string hash,
    string password,
    short keySize = 32,
    int iterations = 10000,
    char splitChar = '.'
    )
    {
        if (string.IsNullOrWhiteSpace(hash) ||
            string.IsNullOrWhiteSpace(password))
            return false;

        password += Configuration.Secrets.PasswordSaltKey;

        var parts = hash.Split(splitChar, 3);

        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out int hashIterations))
            return false;

        if (hashIterations != iterations)
            return false;

        try
        {
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] key = Convert.FromBase64String(parts[2]);

            // Deriva novamente a chave usando o salt original
            byte[] keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                hashIterations,
                HashAlgorithmName.SHA256,
                key.Length
            );

            // Compara as chaves em tempo constante
            return CryptographicOperations.FixedTimeEquals(
                keyToCheck,
                key
            );
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
