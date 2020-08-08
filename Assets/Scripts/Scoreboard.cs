using System;
using System.Collections.Generic;

[Serializable]
public struct CompletedLevel
{
    public int level;
    public float accuracy;
    public DateTime date;
}

[Serializable]
public class Scoreboard
{
    public List<CompletedLevel> completedLevels = new List<CompletedLevel>();
}