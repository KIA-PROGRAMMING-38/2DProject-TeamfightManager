using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ChampFightInfoUI �� �����ϰ� ������ ���̺��� ������ �ٲ� ������ �����ͼ� ChampFightInfoUI�鿡�� ������ �Ѱ��ش�..
/// </summary>
public class ChampFightInfoUIManager : UIBase
{
    [SerializeField] private ChampFightInfoUI ChampFightInfoUIPrefab;

    [SerializeField] private Transform _redTeamUIParent;
    [SerializeField] private Transform _blueTeamUIParent;
	private ChampFightInfoUI[][] fightInfoUIContainer;

    private int _halfChampionCount = 0;

    // Start is called before the first frame update
    IEnumerator Start()
    {
		yield return null;

		SetupChampFightInfoUI();

        // ������ ���̺� �̺�Ʈ ����..
		s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData -= OnChangedChampionBattleData;
		s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData += OnChangedChampionBattleData;

		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanpickData;
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate += UpdateBanpickData;
	}

	private void OnDisable()
	{
        s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData -= OnChangedChampionBattleData;
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanpickData;
	}

    // Champion Fight Info UI ���� �� �ʱ�ȭ..
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
            // ������ �� �����ϰ� �ʱ�ȭ���ش�..
            fightInfoUIContainer[blueTeamIndex][i] = Instantiate<ChampFightInfoUI>(ChampFightInfoUIPrefab, _blueTeamUIParent);
			fightInfoUIContainer[redTeamIndex][i] = Instantiate<ChampFightInfoUI>(ChampFightInfoUIPrefab, _redTeamUIParent);

			fightInfoUIContainer[blueTeamIndex][i].SetBackgroundImage(BattleTeamKind.BlueTeam);
            fightInfoUIContainer[redTeamIndex][i].SetBackgroundImage(BattleTeamKind.RedTeam);

            fightInfoUIContainer[blueTeamIndex][i].SetPilot(dataTable.GetPilot(BattleTeamKind.BlueTeam, i));
            fightInfoUIContainer[redTeamIndex][i].SetPilot(dataTable.GetPilot(BattleTeamKind.RedTeam, i));
		}
	}

    // è�Ǿ��� ��Ʋ ������ �ٲ� �� ȣ��Ǵ� �ݹ� �Լ�..
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
