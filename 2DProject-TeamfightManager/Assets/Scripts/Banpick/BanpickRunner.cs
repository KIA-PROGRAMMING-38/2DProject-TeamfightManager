using System;
using UnityEngine;

public class BanpickRunner : MonoBehaviour
{
	public UIManager uiManager { private get; set; }
	public BattleStageManager battleStageManager { private get; set; }
	public BattleStageDataTable battleStageDataTable { private get; set; }

	private int _curLevel = 0;
	private int _curDetailLevel = 0;
	private int _detailLevelMaxCount = 0;
	private int _levelMaxCount = 0;

	[SerializeField] private BanpickStageGlobalData _globalData;

	private int _curBanStage = 0;
	private int _curPickStage = 0;
	private readonly int BATTLE_TEAM_COUNT = (int)BattleTeamKind.End;

	private void Awake()
	{
		_levelMaxCount = _globalData.stagesDataContainer.Length;
		_detailLevelMaxCount = _globalData.stagesDataContainer[_curLevel].orders.Count;
	}

	public void OnSelectChampion(string championName)
	{
		BanpickStageKind curStageKind = _globalData.stagesDataContainer[_curLevel].kind;
		BattleTeamKind curSelectTeamKind = _globalData.stagesDataContainer[_curLevel].orders[_curDetailLevel];

		switch (curStageKind)
		{
			case BanpickStageKind.Ban:
				battleStageDataTable.UpdateChampionBanData(curSelectTeamKind, _curBanStage / BATTLE_TEAM_COUNT, championName);
				++_curBanStage;
				break;
			case BanpickStageKind.Pick:
				battleStageManager.PickChampion(curSelectTeamKind, _curPickStage / BATTLE_TEAM_COUNT, championName);
				++_curPickStage;
				break;
		}

		++_curDetailLevel;
		if (_detailLevelMaxCount <= _curDetailLevel)
		{
			++_curLevel;
			if (_levelMaxCount <= _curLevel)
			{
				battleStageDataTable.EndBanpick();
			}
			else
			{
				_detailLevelMaxCount = _globalData.stagesDataContainer[_curLevel].orders.Count;
			}
		}
	}
}