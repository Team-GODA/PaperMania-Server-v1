﻿namespace Server.Domain.Entity;

public class PlayerGameData
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = null!;
    public int PlayerExp { get; set; } = 0;
    public int PlayerLevel { get; set; } = 1;
}