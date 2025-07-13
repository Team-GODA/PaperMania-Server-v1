namespace Server.Api.Dto.Response;

public class LoginResponse
{
    public string Message { get; set; } = null!;
    public string SessionId { get; set; } = null!;
}