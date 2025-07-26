namespace Server.Api.Dto.Response;

public class GetPlayerLevelResponse
{
    public int? Id { get; set; }
    public int Level  { get; set; }
    public int Exp { get; set; }
}