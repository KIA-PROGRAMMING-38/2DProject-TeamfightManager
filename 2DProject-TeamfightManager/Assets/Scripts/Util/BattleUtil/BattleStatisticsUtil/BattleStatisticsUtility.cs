﻿using System.Collections.Generic;

public class ChampionBattleStatistics
{
	public int totalPickCount;
	public int totalWinCount;
	public int totalBanCount;
	public int totalAtkDamageAmount;
	public int totalTakeDamageAount;
	public int totalHillAmount;
}

public class TeamBattleStatistics
{
	public Dictionary<string, int> champPickCountContainer;
	public Dictionary<string, int> champBanCountContainer;
	public int totalKillCount;
	public int totalDeathCount;
}

public class PilotBattleStatistics
{
	public Dictionary<string, PilotStageFightData> allStageFightDatas;
	public int totalKillCount;
	public int totalAssistCount;
}

public class BattleTeamFightData
{
	public string teamName;
	public List<BattlePilotFightData> pilotFightDataContainer;
	public List<string> banChampionContainer;
}

public class BattlePilotFightData
{
	public string pilotName;
	public string championName;
	public BattleInfoData battleData;
}

public class PilotStageFightData
{
	public int pickCount;
	public int winCount;
	public int loseCount;
	public int totalAtkDamageAmount;
	public int totalTakeDamageAount;
	public int totalHillAmount;
}
