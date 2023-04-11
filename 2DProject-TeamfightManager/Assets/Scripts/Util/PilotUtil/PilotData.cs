using System.Collections.Generic;
public class ChampionSkillLevelInfo
{
    public string champName;
    public int level;
}

public class PilotData
{
    public string name;
    public int atkStat;
    public int defStat;
    public List<ChampionSkillLevelInfo> champSkillLevelContainer;
}
