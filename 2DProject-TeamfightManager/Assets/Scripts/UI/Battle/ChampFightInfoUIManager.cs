using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampFightInfoUIManager : UIBase
{
    [SerializeField] private ChampFightInfoUI ChampFightInfoUIPrefab;

    [SerializeField] private Transform _redTeamUIParent;
    [SerializeField] private Transform _blueTeamUIParent;
	private ChampFightInfoUI[][] fightInfoUIContainer;

    // Start is called before the first frame update
    void Start()
    {
        SetupChampFightInfoUI();

		s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData -= OnChangedChampionBattleData;
		s_dataTableManager.battleStageDataTable.OnChangedChampionBattleData += OnChangedChampionBattleData;
	}

    private void SetupChampFightInfoUI()
    {
		fightInfoUIContainer = new ChampFightInfoUI[2][];
		for (int i = 0; i < 2; ++i)
		{
            fightInfoUIContainer[i] = new ChampFightInfoUI[4];
		}

        for( int i = 0; i < 4; ++i)
        {
            fightInfoUIContainer[0][i] = Instantiate<ChampFightInfoUI>(ChampFightInfoUIPrefab);
			fightInfoUIContainer[1][i] = Instantiate<ChampFightInfoUI>(ChampFightInfoUIPrefab);

            fightInfoUIContainer[0][i].transform.parent = _blueTeamUIParent;
			fightInfoUIContainer[1][i].transform.parent = _redTeamUIParent;

			fightInfoUIContainer[0][i].SetBackgroundImage(BattleTeamKind.BlueTeam);
            fightInfoUIContainer[1][i].SetBackgroundImage(BattleTeamKind.RedTeam);
		}
	}

    private void OnChangedChampionBattleData(BattleTeamKind teamKind, int index, BattleInfoData data)
    {
        if (index >= 4)
            return;

        fightInfoUIContainer[(int)teamKind][index].UpdateData(data);
    }
}
