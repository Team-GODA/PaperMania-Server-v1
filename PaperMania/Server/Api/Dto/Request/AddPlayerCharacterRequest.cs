namespace Server.Api.Dto.Request;

public class AddPlayerCharacterRequest
{
    public int Id { get; set; }
    public string CharacterId { get; set; } = null!;
}