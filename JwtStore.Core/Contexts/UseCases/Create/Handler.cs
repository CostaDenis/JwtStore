using JwtStore.Core.Contexts.AccountContext.Entities;
using JwtStore.Core.Contexts.AccountContext.ValueObjects;
using JwtStore.Core.Contexts.UseCases.Create.Contracts;

namespace JwtStore.Core.Contexts.UseCases.Create;

public class Handler(IRepository repository, IService service)
{

    private readonly IRepository _repository = repository;
    private readonly IService _service = service;

    public async Task<Response> HandleAsync(
        Request request,
        CancellationToken cancellationToken
    )
    {
        #region Valida a requisição

        try
        {
            var resp = Specification.Assert(request);

            if (!resp.IsValid)
                return new Response("Requisição inválida", 400, resp.Notifications);
        }
        catch (Exception)
        {
            return new Response("Não foi possível validar a requisição", 500);
        }
        #endregion

        #region Gerando objetos

        Email email; ;
        Password password;
        User user;

        try
        {
            email = new Email(request.Email);
            password = new Password(request.Password);
            user = new User(request.Name, email, password);
        }
        catch (Exception ex)
        {
            return new Response(ex.Message, 400);
        }

        #endregion

        #region Verifica se o user já existe

        try
        {
            var exists = await _repository.AnyAsync(request.Email, cancellationToken);

            if (exists)
                return new Response("Esse email já está em uso!", 400);
        }
        catch (Exception)
        {
            return new Response("Falha ao verificar o email cadastrado!", 500);
        }
        #endregion

        #region Persise os dados

        try
        {
            await _repository.SaveAsync(user, cancellationToken);
        }
        catch
        {
            return new Response("Falha ao persistir os dados!", 500);
        }
        #endregion

        #region Envia email de ativação

        try
        {
            await _service.SendVerificationEmailAsync(user, cancellationToken);
        }
        catch
        {
            //Do nothing
        }

        #endregion

        return new Response("Conta criada!", new ResponseData(user.Id, user.Name, user.Email));
    }
}
