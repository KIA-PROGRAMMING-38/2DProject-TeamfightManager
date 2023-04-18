using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StadiumFightUI : UIBase
{
    [SerializeField] private Image _redTeamLogoImage;
    [SerializeField] private TMP_Text _redTeamNameText;
    [SerializeField] private TMP_Text _redTeamTotalKillText;

	[SerializeField] private Image _blueTeamLogoImage;
	[SerializeField] private TMP_Text _blueTeamNameText;
	[SerializeField] private TMP_Text _blueTeamTotalKillText;

	private BattleStageDataTable _battleStageDataTable;

	private void Awake()
	{
		_battleStageDataTable = s_dataTableManager.battleStageDataTable;

		_battleStageDataTable.OnInitializeBattleFightData -= OnInitializeBattleFightData;
		_battleStageDataTable.OnInitializeBattleFightData += OnInitializeBattleFightData;

		_battleStageDataTable.OnChampionDeadEvent -= OnChampionDeadEvent;
		_battleStageDataTable.OnChampionDeadEvent += OnChampionDeadEvent;
	}

	private void OnDisable()
	{
		_battleStageDataTable.OnInitializeBattleFightData -= OnInitializeBattleFightData;
		_battleStageDataTable.OnChampionDeadEvent -= OnChampionDeadEvent;
	}

	private void OnInitializeBattleFightData()
	{
		TeamDataTable teamDataTable = s_dataTableManager.teamDataTable;

		_redTeamNameText.text = _battleStageDataTable.redTeamBattleFightData.teamName;
		_redTeamLogoImage.sprite = teamDataTable.GetLogoSprite(_battleStageDataTable.redTeamBattleFightData.teamName);
		_redTeamTotalKillText.text = StringTable.GetString(0);

		_blueTeamNameText.text = _battleStageDataTable.blueTeamBattleFightData.teamName;
		_blueTeamLogoImage.sprite = teamDataTable.GetLogoSprite(_battleStageDataTable.blueTeamBattleFightData.teamName);
		_blueTeamTotalKillText.text = StringTable.GetString(0);
	}

	private void OnChampionDeadEvent(BattleTeamKind teamKind, int index)
	{
		if (BattleTeamKind.RedTeam == teamKind)
		{
			_blueTeamTotalKillText.text = StringTable.GetString(_battleStageDataTable.blueTeamBattleFightData.teamTotalKill);
		}
		else if (BattleTeamKind.BlueTeam == teamKind)
		{
			_redTeamTotalKillText.text = StringTable.GetString(_battleStageDataTable.redTeamBattleFightData.teamTotalKill);
		}
	}
}
