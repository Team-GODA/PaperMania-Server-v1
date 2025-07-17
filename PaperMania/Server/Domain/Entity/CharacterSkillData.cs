namespace Server.Domain.Entity;

public class CharacterSkillData
{
    public string CharacterId { get; set; } = null!;
    public string SkillId { get; set; } = null!;
    public int SkillLevel { get; set; } = 1;
}