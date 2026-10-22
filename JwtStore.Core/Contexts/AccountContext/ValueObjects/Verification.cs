using JwtStore.Core.Contexts.SharedContext.ValueObjects;

namespace JwtStore.Core.Contexts.AccountContext.ValueObjects;

public class Verification : ValueObject
{

    public Verification()
    {
    }

    public string Code { get; } =
        Guid.NewGuid().ToString("N")[0..6].ToUpper();
    public DateTime? ExpiresAt { get; private set; } = DateTime.UtcNow.AddMinutes(5);
    public DateTime? VerifiedAt { get; private set; } = null;
    public bool IsActive => VerifiedAt != null && ExpiresAt == null;

    public void Verify(string code)
    {
        if (IsActive)
            throw new Exception("Esse item já foi ativado");

        if (ExpiresAt < DateTime.UtcNow)
            throw new Exception("O código já expirou");

        if (!string.Equals(code.Trim(), Code.Trim(), StringComparison.CurrentCultureIgnoreCase))
            throw new Exception("Código de validação inválido");

        ExpiresAt = null;
        VerifiedAt = DateTime.UtcNow;
    }
}
