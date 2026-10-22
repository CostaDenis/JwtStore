namespace JwtStore.Core.Contexts.UseCases.Create;

public record Request(
    string Name,
    string Email,
    string Password);
