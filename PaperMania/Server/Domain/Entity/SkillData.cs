namespace Server.Domain.Entity;

public class SkillData
{
    public string SkillId { get; set; } = null!;
    public string SkillName { get; set; } = null!;
    public SkillType SkillType { get; set; }
}

public enum SkillType
{
    Normal,
    Epic
}