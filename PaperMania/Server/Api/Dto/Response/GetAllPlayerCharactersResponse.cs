using Server.Domain.Entity;

namespace Server.Api.Dto.Response;

public class GetAllPlayerCharactersResponse
{
    public IEnumerable<PlayerCharacterData> PlayerCharacters { get; set; }
}