namespace Server.Domain.Entity;

public class LevelDefinition
{
    public int Level { get; set; }
    public int MaxExp { get; set; } = 0;
    public int MaxActionPoint { get; set; } = 0;
}