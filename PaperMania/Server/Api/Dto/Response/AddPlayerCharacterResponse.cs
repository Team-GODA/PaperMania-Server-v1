using Server.Domain.Entity;

namespace Server.Api.Dto.Response;

public class AddPlayerCharacterResponse
{
    public int Id { get; set; }
    public string CharacterId { get; set; }

    public AddPlayerCharacterResponse(PlayerCharacterData data)
    {
        Id = data.Id;
        CharacterId = data.CharacterId;
    }
}