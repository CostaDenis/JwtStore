using System.Text.RegularExpressions;
using JwtStore.Core.SharedContext.Extensions;
using JwtStore.Core.SharedContext.ValueObjects;

namespace JwtStore.Core.AccountContext.ValueObjects;

public partial class Email : ValueObject
{
    private const string Pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

    protected Email()
    {
    }

    public Email(string address)
    {
        if (string.IsNullOrEmpty(address))
            throw new Exception("Email inválido!");

        var normalizedAddress = address.Trim().ToLower();

        if (normalizedAddress.Length < 5)
            throw new Exception("Email inválido");

        if (!EmailRegex().IsMatch(normalizedAddress))
            throw new Exception("Email inválido");

        Address = normalizedAddress;
    }

    public string Address { get; } = null!;

    //base64 do email
    public string Hash => Address.ToBase64();

    public Verification Verification { get; private set; } = new();

    public void ResendVerification() => Verification = new();

    public static implicit operator string(Email email) => email.ToString();
    public static implicit operator Email(string email) => new(email);

    public override string ToString() => Address.Trim().ToLower();

    [GeneratedRegex(Pattern)]
    private static partial Regex EmailRegex();
}
