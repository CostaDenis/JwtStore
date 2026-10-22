using Flunt.Notifications;
using JwtStore.Core.Contexts.SharedContext.UseCases;

namespace JwtStore.Core.Contexts.UseCases.Create;

public class Response : BaseResponse
{

    protected Response() { }

    public Response(string message, int status,
        IEnumerable<Notification>? notifications = null)
    {
        Message = message;
        Status = status;
        Notifications = notifications;
    }

    public Response(string message, ResponseData data)
    {
        Message = message;
        Status = 201; //Created
        Notifications = null;
        Data = data;
    }

    public ResponseData? Data { get; set; }
}

public record ResponseData(Guid Id, string Name, string Email);
