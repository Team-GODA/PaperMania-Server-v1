namespace Server.Domain.Entity;

public class CharacterSkillData
{
    public string CharacterId { get; set; } = null!;
    public string SkillId { get; set; } = null!;
    public string SkillName { get; set; } = null!;
    public int SkillLevel { get; set; } = 1;
    public string SkillTypeString
    {
        get => SkillType.ToString();
        set => SkillType = Enum.Parse<SkillType>(value);
    }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public SkillType SkillType { get; set; }
}

public enum SkillType
{
    Normal,
    Epic
}