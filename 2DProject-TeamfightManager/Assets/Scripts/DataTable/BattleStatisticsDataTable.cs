using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class BattleStatisticsDataTable
{
	private enum BattleResultKind
	{
		Win,
		Lose,
		Draw
	}

	private Dictionary<string, ChampionBattleStatistics> _champBattleStatisticsContainer;
	private Dictionary<string, TeamBattleStatistics> _teamBattleStatisticsContainer;
	private Dictionary<string, PilotBattleStatistics> _pilotBattleStatisticsContainer;
	private Dictionary<int, PilotBattleStatistics> _dayPilotBattleStatisticsContainer;
	public int totalBattleDayCount { get; private set; }

	public ChampionBattleStatistics GetChampBattleStatistics(string name) => _champBattleStatisticsContainer[name];
	public TeamBattleStatistics GetTeamBattleStatistics(string name) => _teamBattleStatisticsContainer[name];
	public PilotBattleStatistics GetPilotBattleStatistics(string name) => _pilotBattleStatisticsContainer[name];

	public BattleStatisticsDataTable(ChampionDataTable championDataTable, TeamDataTable teamDataTable, PilotDataTable pilotDataTable)
	{
		totalBattleDayCount = 0;

		// 챔피언 관련 정보 초기화..
		int championCount = championDataTable.GetTotalChampionCount();
		_champBattleStatisticsContainer = new Dictionary<string, ChampionBattleStatistics>(championCount);
		for (int i = 0; i < championCount; ++i)
		{
			_champBattleStatisticsContainer.Add(championDataTable.GetChampionName(i), new ChampionBattleStatistics());
		}

		// 팀 관련 정보 초기화..
		int teamCount = teamDataTable.GetTotalTeamCount();
		_teamBattleStatisticsContainer = new Dictionary<string, TeamBattleStatistics>(teamCount);
		for (int i = 0; i < teamCount; ++i)
		{
			_teamBattleStatisticsContainer.Add(teamDataTable.GetTeamName(i), new TeamBattleStatistics());
		}

		// 파일럿 관련 정보 초기화..
		int pilotCount = pilotDataTable.GetTotalPilotCount();
		_pilotBattleStatisticsContainer = new Dictionary<string, PilotBattleStatistics>(pilotCount);
		for (int i = 0; i < pilotCount; ++i)
		{
			_teamBattleStatisticsContainer.Add(pilotDataTable.GetPilotName(i), new TeamBattleStatistics());
		}
	}

	public void AddBattleTeamFightData(BattleTeamFightData redTeamFightData, BattleTeamFightData blueTeamFightData, BattleTeamKind winTeamKind)
	{
		// 팀 관련 통계 정보 갱신..
		BattleResultKind redTeamBattleResultKind = BattleResultKind.Draw;
		BattleResultKind blueTeamBattleResultKind = BattleResultKind.Draw;

		switch (winTeamKind)
		{
			case BattleTeamKind.RedTeam:
				redTeamBattleResultKind = BattleResultKind.Win;
				blueTeamBattleResultKind = BattleResultKind.Lose;
				break;
			case BattleTeamKind.BlueTeam:
				redTeamBattleResultKind = BattleResultKind.Lose;
				blueTeamBattleResultKind = BattleResultKind.Win;
				break;
		}

		UpdateStatisticsData(redTeamFightData, redTeamBattleResultKind);
		UpdateStatisticsData(blueTeamFightData, blueTeamBattleResultKind);

		++totalBattleDayCount;
	}

	private void UpdateStatisticsData(BattleTeamFightData fightData, BattleResultKind resultKind)
	{
		if ( false == _teamBattleStatisticsContainer.ContainsKey( fightData.teamName ) )
			_teamBattleStatisticsContainer.Add( fightData.teamName, new TeamBattleStatistics() );

        TeamBattleStatistics curTeamStatisticsData = _teamBattleStatisticsContainer[fightData.teamName];

		int pilotCount = fightData.pilotFightDataContainer.Count;
		for( int i = 0; i < pilotCount; ++i)
		{
			// 필요한 데이터들을 미리 변수에 담아둔다..
			string pilotName = fightData.pilotFightDataContainer[i].pilotName;
			string championName = fightData.pilotFightDataContainer[i].championName;
			BattleInfoData curBattleData = fightData.pilotFightDataContainer[i].battleData;

			// 파일럿 통계 정보 갱신..
			if ( false == _pilotBattleStatisticsContainer.ContainsKey( pilotName ) )
				_pilotBattleStatisticsContainer.Add( pilotName, new PilotBattleStatistics() );

            PilotBattleStatistics curPilotStatisticsData = _pilotBattleStatisticsContainer[pilotName];

			if (false == curPilotStatisticsData.allStageFightDatas.ContainsKey(championName))
				curPilotStatisticsData.allStageFightDatas.Add(championName, new PilotStageFightData());

			++curPilotStatisticsData.allStageFightDatas[championName].pickCount;
			switch (resultKind)
			{
				case BattleResultKind.Win:
					++curPilotStatisticsData.allStageFightDatas[championName].winCount;
					++curTeamStatisticsData.totalWinCount;
					break;
				case BattleResultKind.Lose:
					++curPilotStatisticsData.allStageFightDatas[championName].winCount;
					++curTeamStatisticsData.totalLoseCount;
					break;
				case BattleResultKind.Draw:
					++curTeamStatisticsData.totalDrawCount;
					break;
			}
			curPilotStatisticsData.allStageFightDatas[championName].totalAtkDamageAmount += curBattleData.totalAtkDamage;
			curPilotStatisticsData.allStageFightDatas[championName].totalTakeDamageAount += curBattleData.totalTakeDamage;
			curPilotStatisticsData.allStageFightDatas[championName].totalHealAmount += curBattleData.totalHeal;
			curPilotStatisticsData.totalKillCount += curBattleData.killCount;
			curPilotStatisticsData.totalAssistCount += curBattleData.assistCount;

            // 챔피언 통계 정보 갱신..
            if ( false == _champBattleStatisticsContainer.ContainsKey( championName ) )
                _champBattleStatisticsContainer.Add( championName, new ChampionBattleStatistics() );

            ChampionBattleStatistics curChampStatisticsData = _champBattleStatisticsContainer[championName];

			++curChampStatisticsData.totalPickCount;
			if (resultKind == BattleResultKind.Win)
				++curChampStatisticsData.totalWinCount;
			curChampStatisticsData.totalAtkDamageAmount += curBattleData.totalAtkDamage;
			curChampStatisticsData.totalTakeDamageAount += curBattleData.totalTakeDamage;
			curChampStatisticsData.totalHealAmount += curBattleData.totalHeal;

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
            if ( false == _champBattleStatisticsContainer.ContainsKey( championName ) )
                _champBattleStatisticsContainer.Add( championName, new ChampionBattleStatistics() );

            ChampionBattleStatistics curChampStatisticsData = _champBattleStatisticsContainer[championName];

			++curChampStatisticsData.totalBanCount;

			// 팀 통계 정보 갱신..
			if (false == curTeamStatisticsData.champBanCountContainer.ContainsKey(championName))
				curTeamStatisticsData.champBanCountContainer.Add(championName, 0);

			++curTeamStatisticsData.champBanCountContainer[championName];
		}
	}
}