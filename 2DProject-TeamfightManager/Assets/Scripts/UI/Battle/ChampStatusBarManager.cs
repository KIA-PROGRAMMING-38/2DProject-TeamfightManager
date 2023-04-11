using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampStatusBarManager : UIBase
{
	public ChampStatusBar ChampStatusBarPrefab;
	private ChampStatusBar[] _champStatusBarContainer;
	private int _champCount;
	private int _halfChampCount;

	private void Awake()
	{
		
	}

	IEnumerator Start()
	{
		yield return null;

		SetupStatusBar();

		s_dataTableManager.battleStageDataTable.OnChangedChampionHPRatio -= OnChangedChampionHPRatio;
		s_dataTableManager.battleStageDataTable.OnChangedChampionHPRatio += OnChangedChampionHPRatio;

		s_dataTableManager.battleStageDataTable.OnChangedChampionMPRatio -= OnChangedChampionMPRatio;
		s_dataTableManager.battleStageDataTable.OnChangedChampionMPRatio += OnChangedChampionMPRatio;

		s_dataTableManager.battleStageDataTable.OnChampionUseUltimate -= OnChampionUseUltimate;
		s_dataTableManager.battleStageDataTable.OnChampionUseUltimate += OnChampionUseUltimate;

		s_dataTableManager.battleStageDataTable.OnChampionDeadEvent -= OnChampionDead;
		s_dataTableManager.battleStageDataTable.OnChampionDeadEvent += OnChampionDead;

		s_dataTableManager.battleStageDataTable.OnChampionRevivalEvent -= OnChampionRevival;
		s_dataTableManager.battleStageDataTable.OnChampionRevivalEvent += OnChampionRevival;
	}

	private void OnDisable()
	{
		s_dataTableManager.battleStageDataTable.OnChangedChampionHPRatio -= OnChangedChampionHPRatio;
		s_dataTableManager.battleStageDataTable.OnChangedChampionMPRatio -= OnChangedChampionMPRatio;
		s_dataTableManager.battleStageDataTable.OnChampionUseUltimate -= OnChampionUseUltimate;
		s_dataTableManager.battleStageDataTable.OnChampionDeadEvent -= OnChampionDead;
		s_dataTableManager.battleStageDataTable.OnChampionRevivalEvent -= OnChampionRevival;
	}

	private void SetupStatusBar()
	{
		BattleStageDataTable dataTable = s_dataTableManager.battleStageDataTable;

		_champCount = dataTable.battleChampionTotalCount;
		_halfChampCount = _champCount / 2;

		_champStatusBarContainer = new ChampStatusBar[_champCount];
		for( int i = 0; i < _champCount; ++i)
		{
			BattleTeamKind teamKind = (BattleTeamKind)(i / _halfChampCount);
			int teamIndex = (i % _halfChampCount);

			_champStatusBarContainer[i] = Instantiate<ChampStatusBar>(ChampStatusBarPrefab);
			_champStatusBarContainer[i].transform.parent = transform;
			_champStatusBarContainer[i].target = dataTable.GetChampionTransform(teamKind, teamIndex);
			_champStatusBarContainer[i].teamKind = teamKind;
			_champStatusBarContainer[i].SetUltimateIconSprite(dataTable.GetChampionUltimateIconSprite(teamKind, teamIndex));
		}
	}

	public void OnChangedChampionHPRatio(BattleTeamKind teamKind, int index, float hpRatio)
	{
		if (_champCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[teamIndex * _halfChampCount + index].SetHPRatio(hpRatio);
	}

	public void OnChangedChampionMPRatio(BattleTeamKind teamKind, int index, float mpRatio)
	{
		if (_champCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[teamIndex * _halfChampCount + index].SetMPRatio(mpRatio);
	}

	public void OnChampionUseUltimate(BattleTeamKind teamKind, int index)
	{
		if (_champCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[teamIndex * _halfChampCount + index].SetUltimateActive(false);
	}

	public void OnChampionDead(BattleTeamKind teamKind, int index)
	{
		int teamIndex = (int)teamKind;

		_champStatusBarContainer[teamIndex * _halfChampCount + index].gameObject.SetActive(false);
	}

	public void OnChampionRevival(BattleTeamKind teamKind, int index)
	{
		int teamIndex = (int)teamKind;

		_champStatusBarContainer[teamIndex * _halfChampCount + index].gameObject.SetActive(true);
	}
}
