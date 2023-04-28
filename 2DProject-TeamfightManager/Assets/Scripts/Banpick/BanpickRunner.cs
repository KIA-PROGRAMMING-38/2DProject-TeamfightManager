using System;
using System.Collections;
using UnityEngine;

public class BanpickRunner : MonoBehaviour
{
	public GameManager gameManager
	{
		set
		{
			_battleStageManager = value.battleStageManager;
			_battleStageDataTable = value.dataTableManager.battleStageDataTable;

			_globalData = value.gameGlobalData.banpickStageGlobalData;
			_levelMaxCount = _globalData.stagesDataContainer.Length;
			_detailLevelMaxCount = _globalData.stagesDataContainer[_curLevel].orders.Count;

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
	private int _progressStageCount = 1;
	private int _levelMaxCount = 0;

	[SerializeField] private BanpickStageGlobalData _globalData;

	private int _curBanStage = 0;
	private int _curPickStage = 0;
	private readonly int BATTLE_TEAM_COUNT = (int)BattleTeamKind.End;

	BanpickStageKind _curStageKind;
	BattleTeamKind _curSelectTeamKind;

	private void Start()
	{
		_battleStageDataTable.StartBanpick(_globalData.stagesDataContainer[0].kind, _globalData.stagesDataContainer[0].orders[0]);

		StartCoroutine(DelayBanpickStart());
	}

	IEnumerator DelayBanpickStart()
	{
		yield return YieldInstructionStore.GetWaitForSec(1f);

		ProgressBanpick();
	}

	public void OnSelectChampion(string championName)
	{
		BanpickStageKind tmpCurStageKind = _curStageKind;
		BattleTeamKind tmpCurSelectTeamKind = _curSelectTeamKind;
		int curStage = 0;

		switch (_curStageKind)
		{
			case BanpickStageKind.Ban:
				curStage = _curBanStage / BATTLE_TEAM_COUNT;
				++_curBanStage;
				break;
			case BanpickStageKind.Pick:
				curStage = _curPickStage / BATTLE_TEAM_COUNT;
				++_curPickStage;
				_battleStageManager.PickChampion(tmpCurSelectTeamKind, curStage, championName);
				break;
		}

		Debug.Log("BanPick");

		++_progressStageCount;
		++_curDetailLevel;
		if (_detailLevelMaxCount <= _curDetailLevel)
		{
			++_curLevel;
			if (_levelMaxCount <= _curLevel)
			{
				SetReceiveButtonEventState(false);
				_battleStageDataTable.EndBanpick();
			}
			else
			{
				_detailLevelMaxCount = _globalData.stagesDataContainer[_curLevel].orders.Count;
				_curDetailLevel = 0;

				_battleStageDataTable.StartBanpickOneStage(_globalData.stagesDataContainer[_curLevel].kind);
			}
		}

		if (_levelMaxCount > _curLevel)
		{
			_battleStageDataTable.curBanpickStageInfo.Set(
				_globalData.stagesDataContainer[_curLevel].kind,
				_globalData.stagesDataContainer[_curLevel].orders[_curDetailLevel], curStage, _progressStageCount);

			ProgressBanpick();
		}

		switch (tmpCurStageKind)
		{
			case BanpickStageKind.Ban:
				_battleStageDataTable.UpdateBanpickData(championName, tmpCurStageKind, tmpCurSelectTeamKind, curStage);
				break;
			case BanpickStageKind.Pick:
				_battleStageDataTable.UpdateBanpickData(championName, tmpCurStageKind, tmpCurSelectTeamKind, curStage);
				break;
		}
	}

	public void ProgressBanpick()
	{
		StartCoroutine(CheckPauseBanpick());
	}

	IEnumerator CheckPauseBanpick()
	{
		while (_battleStageDataTable.isPauseBanpick)
		{
			yield return null;
		}

		_curStageKind = _globalData.stagesDataContainer[_curLevel].kind;
		_curSelectTeamKind = _globalData.stagesDataContainer[_curLevel].orders[_curDetailLevel];

		_battleStageManager.ProgressBanpick(this, _curSelectTeamKind);
	}

	// 버튼 이벤트를 받을건지 안받을건지 인자의 값에 따라 정하는 함수..
	public void SetReceiveButtonEventState(bool isOnReceive)
	{
		if(true == isOnReceive)
		{
			_battleStageDataTable.OnClickedSelectChampionButton -= OnSelectChampion;
			_battleStageDataTable.OnClickedSelectChampionButton += OnSelectChampion;
		}
		else
		{
			_battleStageDataTable.OnClickedSelectChampionButton -= OnSelectChampion;
		}
	}
}