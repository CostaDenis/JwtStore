using System.Diagnostics.Contracts;
using Flunt.Notifications;
using Flunt.Validations;

namespace JwtStore.Core.Contexts.UseCases.Create;

//Contrato para garantir que a requisição seja válida
public static class Specification
{

    public static Contract<Notification> Assert(Request request)
        => new Contract<Notification>()
            .Requires()
            .IsLowerThan(request.Name.Length, 120, "Name", "O nome deve ter até 120 caracteres!")
            .IsGreaterThan(request.Name.Length, 3, "Name", "O nome deve ter pelo menos 3 caracteres!")
            .IsLowerThan(request.Password.Length, 40, "Password", "A senha deve ter até 40 caracteres!")
            .IsGreaterThan(request.Password.Length, 8, "Password", "A senha deve ter pelo menos 8 caracteres!")
            .IsEmail(request.Email, "Email", "Email inválido!");
}
