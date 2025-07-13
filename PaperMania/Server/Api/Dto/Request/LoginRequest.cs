namespace Server.Api.Dto.Request;

public class LoginRequest
{
    public string PlayerId { get; set; } = null!;
    public string Password { get; set; } = null!;
}