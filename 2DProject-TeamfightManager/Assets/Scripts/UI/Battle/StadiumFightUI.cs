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

	[SerializeField] private TMP_Text _remainTimeText;
	[SerializeField] private TMP_Text _symbolText;

    private BattleStageDataTable _battleStageDataTable;

	private void Awake()
	{
		_battleStageDataTable = s_dataTableManager.battleStageDataTable;

		_battleStageDataTable.OnChampionDeadEvent -= OnChampionDeadEvent;
		_battleStageDataTable.OnChampionDeadEvent += OnChampionDeadEvent;

		_battleStageDataTable.OnStartBattle -= OnStartBattle;
		_battleStageDataTable.OnStartBattle += OnStartBattle;

		_battleStageDataTable.OnUpdateBattleRemainTime -= OnUpdateBattleRemainTime;
		_battleStageDataTable.OnUpdateBattleRemainTime += OnUpdateBattleRemainTime;
    }

    private void Start()
    {
		InitializeBattleFightData();

        _redTeamTotalKillText.gameObject.SetActive( false );
        _blueTeamTotalKillText.gameObject.SetActive( false );
		_remainTimeText.gameObject.SetActive( false );
		_symbolText.gameObject.SetActive( false );
    }

    private void OnDisable()
	{
		_battleStageDataTable.OnChampionDeadEvent -= OnChampionDeadEvent;
        _battleStageDataTable.OnStartBattle -= OnStartBattle;
        _battleStageDataTable.OnUpdateBattleRemainTime -= OnUpdateBattleRemainTime;
    }

	private void InitializeBattleFightData()
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

	private void OnStartBattle()
	{
        _redTeamTotalKillText.gameObject.SetActive( true );
        _blueTeamTotalKillText.gameObject.SetActive( true );

		_remainTimeText.gameObject.SetActive( true );
		_symbolText.gameObject.SetActive( true );
    }

	private void OnUpdateBattleRemainTime(float time)
	{
		_remainTimeText.text = StringTable.GetString( (int)time );
    }
}
