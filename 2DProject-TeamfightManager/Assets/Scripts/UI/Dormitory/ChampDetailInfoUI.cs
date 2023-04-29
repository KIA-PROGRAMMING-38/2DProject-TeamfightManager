using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChampDetailInfoUI : UIBase
{
    private ShowChampDataUI _showChampDataUI;
	private ShowChampStatusUI _showChampStatusUI;
	private TMP_Text _champDescText;
	private ChampDetailInfo_BattleStatisticsUI _battleStatisticsUI;

	private void Awake()
	{
		_showChampDataUI = GetComponentInChildren<ShowChampDataUI>();
		_showChampStatusUI = GetComponentInChildren<ShowChampStatusUI>();
		_champDescText = transform.Find("ChampInfo").Find("ChampDescText").GetComponent<TMP_Text>();
		_battleStatisticsUI = GetComponentInChildren<ChampDetailInfo_BattleStatisticsUI>();
	}

	// 챔피언 이름을 받아와 그 챔피언의 정보를 출력하도록 UI 초기화 진행..
	public void ShowChampionDetailInfo(string champName)
    {
		ChampionStatus status = s_dataTableManager.championDataTable.GetChampionStatus(champName);

		_showChampDataUI.ChangeChampionData(champName);
		_showChampStatusUI.status = status;

		_champDescText.text = s_dataTableManager.championDataTable.GetChampionData(champName).champDescription;

		_battleStatisticsUI.showChampName = champName;
	}
}
