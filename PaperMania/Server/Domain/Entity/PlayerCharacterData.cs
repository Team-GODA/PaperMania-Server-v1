namespace Server.Domain.Entity;

public class PlayerCharacterData
{
    public int Id { get; set; }
    public string CharacterId { get; set; } = null!;
    public string CharacterName { get; set; } = null!;
    public int CharacterLevel { get; set; } = 1;
}