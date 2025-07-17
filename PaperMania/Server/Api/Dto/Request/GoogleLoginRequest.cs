namespace Server.Api.Dto.Request;

public class GoogleLoginRequest
{
    public string IdToken { get; set; } = null!;
    public string PlayerId { get; set; } = null!;
    public string Password { get; set; } = null!;
}