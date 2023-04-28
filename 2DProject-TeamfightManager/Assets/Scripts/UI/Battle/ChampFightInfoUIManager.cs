using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 모든 ChampFightInfoUI 를 관리하고 데이터 테이블의 정보가 바뀔 때마다 가져와서 ChampFightInfoUI들에게 정보를 넘겨준다..
/// </summary>
public class ChampFightInfoUIManager : UIBase
{
    [SerializeField] private ChampFightInfoUI ChampFightInfoUIPrefab;

    [SerializeField] private Transform _redTeamUIParent;
    [SerializeField] private Transform _blueTeamUIParent;
	private ChampFightInfoUI[][] fightInfoUIContainer;

    private int _halfChampionCount = 0;

    IEnumerator Start()
    {
		yield return null;

		SetupChampFightInfoUI();

        // 데이터 테이블 이벤트 구독..
		s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData -= OnChangedChampionBattleData;
		s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData += OnChangedChampionBattleData;

		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanpickData;
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate += UpdateBanpickData;

        s_dataTableManager.battleStageDataTable.OnStartBattle -= OnStartBattle;
        s_dataTableManager.battleStageDataTable.OnStartBattle += OnStartBattle;

        s_dataTableManager.battleStageDataTable.OnBattleEnd -= OnBattleEnd;
        s_dataTableManager.battleStageDataTable.OnBattleEnd += OnBattleEnd;
    }

	private void OnStartBattle()
	{
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanpickData;
        s_dataTableManager.battleStageDataTable.OnStartBattle -= OnStartBattle;
    }

    private void OnBattleEnd(BattleTeam redTeam, BattleTeam blueTeam, BattleTeamKind winTeamKind)
    {
        s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData -= OnChangedChampionBattleData;
        s_dataTableManager.battleStageDataTable.OnBattleEnd -= OnBattleEnd;
    }

    // Champion Fight Info UI 생성 및 초기화..
    private void SetupChampFightInfoUI()
    {
		BattleStageDataTable dataTable = s_dataTableManager.battleStageDataTable;

		_halfChampionCount = dataTable.battleChampionTotalCount / 2;

		fightInfoUIContainer = new ChampFightInfoUI[(int)BattleTeamKind.End][];
		for (int i = 0; i < 2; ++i)
		{
            fightInfoUIContainer[i] = new ChampFightInfoUI[_halfChampionCount];
		}

        int redTeamIndex = (int)BattleTeamKind.RedTeam;
        int blueTeamIndex = (int)BattleTeamKind.BlueTeam;
		for ( int i = 0; i < _halfChampionCount; ++i)
        {
            // 생성한 뒤 적절하게 초기화해준다..
            fightInfoUIContainer[blueTeamIndex][i] = Instantiate<ChampFightInfoUI>(ChampFightInfoUIPrefab, _blueTeamUIParent);
			fightInfoUIContainer[redTeamIndex][i] = Instantiate<ChampFightInfoUI>(ChampFightInfoUIPrefab, _redTeamUIParent);

			fightInfoUIContainer[blueTeamIndex][i].SetBackgroundImage(BattleTeamKind.BlueTeam);
            fightInfoUIContainer[redTeamIndex][i].SetBackgroundImage(BattleTeamKind.RedTeam);

			fightInfoUIContainer[blueTeamIndex][i].SetPilot(dataTable.GetPilot(BattleTeamKind.BlueTeam, i));
			fightInfoUIContainer[redTeamIndex][i].SetPilot(dataTable.GetPilot(BattleTeamKind.RedTeam, i));
		}
	}

    // 챔피언의 배틀 정보가 바뀔 시 호출되는 콜백 함수..
    private void OnChangedChampionBattleData(BattleTeamKind teamKind, int index, BattleInfoData data)
    {
        if (index >= _halfChampionCount)
            return;

        fightInfoUIContainer[(int)teamKind][index].UpdateData(data);
    }

    private void UpdateBanpickData(string champName, BanpickStageKind banpickKind, BattleTeamKind teamKind, int index)
    {
        if (BanpickStageKind.Pick != banpickKind)
            return;

        fightInfoUIContainer[(int)teamKind][index].ChangePilotImageToChampImage(champName);
    }
}
