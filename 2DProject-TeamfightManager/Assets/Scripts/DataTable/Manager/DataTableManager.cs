using UnityEngine;

/// <summary>
/// 데이터 테이블을 관리할 매니저 클래스..
/// </summary>
public class DataTableManager : MonoBehaviour
{
	public ChampionDataTable championDataTable { get; private set; }
	public PilotDataTable pilotDataTable { get; private set; }
	public EffectDataTable effectDataTable { get; private set; }
	public AttackActionDataTable attackActionDataTable { get; private set; }
	public BattleStageDataTable battleStageDataTable { get; private set; }
	public TeamDataTable teamDataTable { get; private set; }
	public BattleStatisticsDataTable statisticsDataTable { get; private set; }

	private void Awake()
	{
		Champion.s_dataTableManager = this;
		UIBase.s_dataTableManager = this;

		championDataTable = new ChampionDataTable();
		pilotDataTable = new PilotDataTable();
		effectDataTable = new EffectDataTable();
		attackActionDataTable = new AttackActionDataTable();
		battleStageDataTable = new BattleStageDataTable();
		teamDataTable = new TeamDataTable();
	}

	public void OnEndReadData()
	{
		statisticsDataTable = new BattleStatisticsDataTable(championDataTable, teamDataTable, pilotDataTable);
		battleStageDataTable.championDataTable = championDataTable;
	}
}
