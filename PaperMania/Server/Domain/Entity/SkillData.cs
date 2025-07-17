namespace Server.Domain.Entity;

public class SkillData
{
    public string SkillId { get; set; } = null!;
    public string SkillName { get; set; } = null!;
    
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