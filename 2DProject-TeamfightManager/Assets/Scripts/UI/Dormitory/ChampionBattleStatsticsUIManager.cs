using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionBattleStatsticsUIManager : UIBase
{
	[SerializeField] private ChampionBattleStatsticsUI _uiPrefab;
	[SerializeField] private RectTransform _scroolViewContentTransform;
	private ChampionBattleStatsticsUI[] _uiContainer;

	private void Awake()
	{
		int totalChampionCount = s_dataTableManager.championDataTable.GetTotalChampionCount();
		_uiContainer = new ChampionBattleStatsticsUI[totalChampionCount];

		for( int i = 0; i < totalChampionCount; ++i)
		{
			_uiContainer[i] = Instantiate(_uiPrefab, _scroolViewContentTransform);
			_uiContainer[i].ranking = i + 1;
			_uiContainer[i].showChampionName = s_dataTableManager.championDataTable.GetChampionName(i);
		}

		_scroolViewContentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (totalChampionCount + 1) * 50);
	}
}
