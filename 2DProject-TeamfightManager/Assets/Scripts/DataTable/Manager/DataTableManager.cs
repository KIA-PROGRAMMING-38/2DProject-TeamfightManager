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