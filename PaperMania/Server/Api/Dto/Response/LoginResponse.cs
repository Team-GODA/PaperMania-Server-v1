namespace Server.Api.Dto.Response;

public class LoginResponse
{
    public string Message { get; set; } = null!;
    public int Id { get; set; }
    public string SessionId { get; set; } = null!;
}