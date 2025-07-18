namespace Server.Api.Dto.Response;

public class GoogleLoginResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string? Message { get; set; }
}