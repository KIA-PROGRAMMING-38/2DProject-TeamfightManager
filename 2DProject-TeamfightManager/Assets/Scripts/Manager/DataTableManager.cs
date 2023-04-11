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

	private void Awake()
	{
		championDataTable = new ChampionDataTable();
		pilotDataTable = new PilotDataTable();
		effectDataTable = new EffectDataTable();
		attackActionDataTable = new AttackActionDataTable();
		battleStageDataTable = new BattleStageDataTable();

		battleStageDataTable.championDataTable = championDataTable;

		Champion.s_dataTableManager = this;
	}
}
