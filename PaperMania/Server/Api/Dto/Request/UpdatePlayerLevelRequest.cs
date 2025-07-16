namespace Server.Api.Dto.Request;

public class UpdatePlayerLevelRequest
{
    public int Id { get; set; }
    public int NewLevel { get; set; }
    public int NewExp { get; set; }
}