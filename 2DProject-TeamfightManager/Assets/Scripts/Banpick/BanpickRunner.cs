using System;
using UnityEngine;

public class BanpickRunner : MonoBehaviour
{
	public GameManager gameManager
	{
		set
		{
			_battleStageManager = value.battleStageManager;
			_battleStageDataTable = value.dataTableManager.battleStageDataTable;

			_battleStageDataTable.OnClickedSelectChampionButton -= OnSelectChampion;
			_battleStageDataTable.OnClickedSelectChampionButton += OnSelectChampion;

			_globalData = value.gameGlobalData.banpickStageGlobalData;
			_levelMaxCount = _globalData.stagesDataContainer.Length;
			_detailLevelMaxCount = _globalData.stagesDataContainer[_curLevel].orders.Count;

			_battleStageDataTable.StartBanpick(_globalData.stagesDataContainer[0].kind, _globalData.stagesDataContainer[0].orders[0]);

			int banpickLevelMaxCount = 0;
			int totalBanChampCount = 0;
			for ( int i = 0; i < _levelMaxCount; ++i)
			{
				banpickLevelMaxCount += _globalData.stagesDataContainer[i].orders.Count;
				if (_globalData.stagesDataContainer[i].kind == BanpickStageKind.Ban)
					totalBanChampCount += _globalData.stagesDataContainer[i].orders.Count / 2;
			}

			_battleStageDataTable.maxBanpickLevel = banpickLevelMaxCount;
			_battleStageDataTable.totalBanChampCount = totalBanChampCount;
        }
	}

	private BattleStageManager _battleStageManager;
	private BattleStageDataTable _battleStageDataTable;

	private int _curLevel = 0;
	private int _curDetailLevel = 0;
	private int _detailLevelMaxCount = 0;
	private int _levelMaxCount = 0;

	[SerializeField] private BanpickStageGlobalData _globalData;

	private int _curBanStage = 0;
	private int _curPickStage = 0;
	private readonly int BATTLE_TEAM_COUNT = (int)BattleTeamKind.End;

	public void OnSelectChampion(string championName)
	{
		BanpickStageKind curStageKind = _globalData.stagesDataContainer[_curLevel].kind;
		BattleTeamKind curSelectTeamKind = _globalData.stagesDataContainer[_curLevel].orders[_curDetailLevel];

		switch (curStageKind)
		{
			case BanpickStageKind.Ban:
				_battleStageDataTable.UpdateBanpickData(championName, curStageKind, curSelectTeamKind, _curBanStage / BATTLE_TEAM_COUNT);
				++_curBanStage;
				break;
			case BanpickStageKind.Pick:
				_battleStageDataTable.UpdateBanpickData(championName, curStageKind, curSelectTeamKind, _curPickStage / BATTLE_TEAM_COUNT);
				_battleStageManager.PickChampion(curSelectTeamKind, _curPickStage / BATTLE_TEAM_COUNT, championName);
				++_curPickStage;
				break;
		}

		++_curDetailLevel;
		if (_detailLevelMaxCount <= _curDetailLevel)
		{
			++_curLevel;
			if (_levelMaxCount <= _curLevel)
			{
                _battleStageDataTable.OnClickedSelectChampionButton -= OnSelectChampion;
                _battleStageDataTable.EndBanpick();

				return;
			}
			else
			{
				_detailLevelMaxCount = _globalData.stagesDataContainer[_curLevel].orders.Count;
				_curDetailLevel = 0;

				_battleStageDataTable.StartBanpickOneStage(_globalData.stagesDataContainer[_curLevel].kind);
			}
		}

		int level = (_globalData.stagesDataContainer[_curLevel].kind == BanpickStageKind.Pick)
			? _curPickStage / BATTLE_TEAM_COUNT : _curBanStage / BATTLE_TEAM_COUNT;

		_battleStageDataTable.curBanpickStageInfo.Set(_globalData.stagesDataContainer[_curLevel].kind,
			_globalData.stagesDataContainer[_curLevel].orders[_curDetailLevel], level);
	}
}