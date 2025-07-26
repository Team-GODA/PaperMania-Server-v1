using Server.Domain.Entity;

namespace Server.Api.Dto.Response;

public class AddPlayerCharacterResponse
{
    public int Id { get; set; }
    public string CharacterId { get; set; }
}