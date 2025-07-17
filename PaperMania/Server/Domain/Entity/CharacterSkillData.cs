namespace Server.Domain.Entity;

public class CharacterSkillData
{
    public int Id { get; set; }
    public string CharacterId { get; set; } = null!;
    public int NormalSkillLevel { get; set; } = 0;
    public int EpicSkillLevel { get; set; } = 0;
}