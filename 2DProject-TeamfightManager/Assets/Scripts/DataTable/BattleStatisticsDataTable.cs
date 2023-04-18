using System.Collections.Generic;

public class BattleStatisticsDataTable
{
	public ChampionDataTable championDataTable
	{
		set
		{
			int championCount = value.GetTotalChampionCount();
			_champBattleStatisticsContainer = new Dictionary<string, ChampionBattleStatistics>(championCount);
			for (int i = 0; i < championCount; ++i)
			{
				_champBattleStatisticsContainer.Add(value.GetChampionName(i), new ChampionBattleStatistics());
			}
		}
	}

	public TeamDataTable teamDataTable
	{
		set
		{
			int teamCount = value.GetTotalTeamCount();
			_teamBattleStatisticsContainer = new Dictionary<string, TeamBattleStatistics>(teamCount);
			for (int i = 0; i < teamCount; ++i)
			{
				_teamBattleStatisticsContainer.Add(value.GetTeamName(i), new TeamBattleStatistics());
			}
		}
	}

	public PilotDataTable pilotDataTable
	{
		set
		{
			int pilotCount = value.GetTotalPilotCount();
			_pilotBattleStatisticsContainer = new Dictionary<string, PilotBattleStatistics>(pilotCount);
			for (int i = 0; i < pilotCount; ++i)
			{
				_teamBattleStatisticsContainer.Add(value.GetPilotName(i), new TeamBattleStatistics());
			}
		}
	}

	private Dictionary<string, ChampionBattleStatistics> _champBattleStatisticsContainer;
	private Dictionary<string, TeamBattleStatistics> _teamBattleStatisticsContainer;
	private Dictionary<string, PilotBattleStatistics> _pilotBattleStatisticsContainer;
	private Dictionary<int, PilotBattleStatistics> _dayPilotBattleStatisticsContainer;

	public void AddBattleTeamFightData(BattleTeamFightData redTeamFightData, BattleTeamFightData blueTeamFightData, BattleTeamKind winTeamKind)
	{
		bool isRedTeamwin = (winTeamKind == BattleTeamKind.RedTeam) ? true : false;

		// 팀 관련 통계 정보 갱신..
		UpdateStatisticsData(redTeamFightData, isRedTeamwin);
		UpdateStatisticsData(blueTeamFightData, !isRedTeamwin);
	}

	private void UpdateStatisticsData(BattleTeamFightData fightData, bool isWinTeam)
	{
		TeamBattleStatistics curTeamStatisticsData = _teamBattleStatisticsContainer[fightData.teamName];

		int pilotCount = fightData.pilotFightDataContainer.Count;
		for( int i = 0; i < pilotCount; ++i)
		{
			// 필요한 데이터들을 미리 변수에 담아둔다..
			string pilotName = fightData.pilotFightDataContainer[i].pilotName;
			string championName = fightData.pilotFightDataContainer[i].championName;
			BattleInfoData curBattleData = fightData.pilotFightDataContainer[i].battleData;

			// 파일럿 통계 정보 갱신..
			PilotBattleStatistics curPilotStatisticsData = _pilotBattleStatisticsContainer[pilotName];

			if (false == curPilotStatisticsData.allStageFightDatas.ContainsKey(championName))
				curPilotStatisticsData.allStageFightDatas.Add(championName, new PilotStageFightData());

			++curPilotStatisticsData.allStageFightDatas[championName].pickCount;
			if (true == isWinTeam)
				++curPilotStatisticsData.allStageFightDatas[championName].winCount;
			else
				++curPilotStatisticsData.allStageFightDatas[championName].loseCount;
			curPilotStatisticsData.allStageFightDatas[championName].totalAtkDamageAmount += curBattleData.totalAtkDamage;
			curPilotStatisticsData.allStageFightDatas[championName].totalTakeDamageAount += curBattleData.totalTakeDamage;
			curPilotStatisticsData.allStageFightDatas[championName].totalHillAmount += curBattleData.totalHill;
			curPilotStatisticsData.totalKillCount += curBattleData.killCount;
			curPilotStatisticsData.totalAssistCount += curBattleData.assistCount;

			// 챔피언 통계 정보 갱신..
			ChampionBattleStatistics curChampStatisticsData = _champBattleStatisticsContainer[championName];

			++curChampStatisticsData.totalPickCount;
			if (isWinTeam)
				++curChampStatisticsData.totalWinCount;
			curChampStatisticsData.totalAtkDamageAmount += curBattleData.totalAtkDamage;
			curChampStatisticsData.totalTakeDamageAount += curBattleData.totalTakeDamage;
			curChampStatisticsData.totalHillAmount += curBattleData.totalHill;

			// 팀 통계 정보 갱신..
			curTeamStatisticsData.totalKillCount += curBattleData.killCount;
			curTeamStatisticsData.totalDeathCount += curBattleData.deathCount;

			if (false == curTeamStatisticsData.champPickCountContainer.ContainsKey(championName))
				curTeamStatisticsData.champPickCountContainer.Add(championName, 0);

			++curTeamStatisticsData.champPickCountContainer[championName];
		}

		// 밴 관련 통계 정보 갱신..
		int banChampCount = fightData.banChampionContainer.Count;
		for( int i = 0; i < banChampCount; ++i)
		{
			string championName = fightData.banChampionContainer[i];

			// 챔피언 통계 정보 갱신..
			ChampionBattleStatistics curChampStatisticsData = _champBattleStatisticsContainer[championName];

			++curChampStatisticsData.totalBanCount;

			// 팀 통계 정보 갱신..
			if (false == curTeamStatisticsData.champBanCountContainer.ContainsKey(championName))
				curTeamStatisticsData.champBanCountContainer.Add(championName, 0);

			++curTeamStatisticsData.champBanCountContainer[championName];
		}
	}
}