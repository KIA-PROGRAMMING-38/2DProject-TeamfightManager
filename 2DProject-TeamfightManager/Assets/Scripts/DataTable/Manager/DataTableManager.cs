using UnityEngine;

/// <summary>
/// ������ ���̺��� ������ �Ŵ��� Ŭ����..
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
		statisticsDataTable = new BattleStatisticsDataTable();

		battleStageDataTable.championDataTable = championDataTable;

		statisticsDataTable.championDataTable = championDataTable;
		statisticsDataTable.teamDataTable = teamDataTable;
		statisticsDataTable.pilotDataTable = pilotDataTable;
	}
}
