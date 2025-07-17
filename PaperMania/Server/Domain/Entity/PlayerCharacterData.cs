namespace Server.Domain.Entity;

public class PlayerCharacterData
{
    public int Id { get; set; }
    public string CharacterId { get; set; } = null!;
    public string CharacterName { get; set; } = null!;
    public int CharacterLevel { get; set; } = 1;
    public string RarityString
    {
        get => Rarity.ToString();
        set => Rarity = Enum.Parse<Rarity>(value);
    }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Rarity Rarity { get; set; }
}