using System.Collections.Generic;

[System.Serializable]
public class ChampionSkillLevelInfo
{
    public string champName;
    public int level;
}

[System.Serializable]
public class PilotData
{
    public string name;
    public int atkStat;
    public int defStat;
    public List<ChampionSkillLevelInfo> champSkillLevelContainer;
}
