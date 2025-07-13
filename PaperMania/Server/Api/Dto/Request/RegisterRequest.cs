namespace Server.Api.Dto.Request;

public class RegisterRequest
{
    public string PlayerId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}