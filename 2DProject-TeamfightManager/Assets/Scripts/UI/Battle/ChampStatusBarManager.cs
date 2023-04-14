using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ChampStatusBar를 관리하며 필요한 정보 및 변하는 정보들을 가져와 ChampStatusBar들에게 넘겨준다..
/// </summary>
public class ChampStatusBarManager : UIBase
{
	public ChampStatusBar ChampStatusBarPrefab;
	private ChampStatusBar[][] _champStatusBarContainer;
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

		s_dataTableManager.battleStageDataTable.OnChangedChampionBarrierRatio -= OnChangedChampionBarrierRatio;
		s_dataTableManager.battleStageDataTable.OnChangedChampionBarrierRatio += OnChangedChampionBarrierRatio;

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
		s_dataTableManager.battleStageDataTable.OnChangedChampionBarrierRatio -= OnChangedChampionBarrierRatio;
		s_dataTableManager.battleStageDataTable.OnChampionDeadEvent -= OnChampionDead;
		s_dataTableManager.battleStageDataTable.OnChampionRevivalEvent -= OnChampionRevival;
	}

	private void SetupStatusBar()
	{
		BattleStageDataTable dataTable = s_dataTableManager.battleStageDataTable;
		RectTransform myUITransform = GetComponent<RectTransform>();

		_halfChampCount = dataTable.battleChampionTotalCount / 2;

		_champStatusBarContainer = new ChampStatusBar[(int)BattleTeamKind.End][];
		for( int i = 0; i < (int)BattleTeamKind.End; ++i)
		{
			BattleTeamKind teamKind = (BattleTeamKind)i;
			_champStatusBarContainer[i] = new ChampStatusBar[_halfChampCount];

			for ( int j = 0; j < _halfChampCount; ++j)
			{
				ChampStatusBar newChampStatusBar = null;

				newChampStatusBar = Instantiate<ChampStatusBar>(ChampStatusBarPrefab, myUITransform);
				newChampStatusBar.target = dataTable.GetChampionTransform(teamKind, j);
				newChampStatusBar.teamKind = teamKind;
				newChampStatusBar.SetUltimateIconSprite(dataTable.GetChampionUltimateIconSprite(teamKind, j));

				_champStatusBarContainer[i][j] = newChampStatusBar;
			}
		}
	}

	public void OnChangedChampionHPRatio(BattleTeamKind teamKind, int index, float hpRatio)
	{
		if (_halfChampCount <= index)
			return;

		_champStatusBarContainer[(int)teamKind][index].SetHPRatio(hpRatio);
	}

	public void OnChangedChampionMPRatio(BattleTeamKind teamKind, int index, float mpRatio)
	{
		if (_halfChampCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[(int)teamKind][index].SetMPRatio(mpRatio);
	}

	public void OnChampionUseUltimate(BattleTeamKind teamKind, int index)
	{
		if (_halfChampCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[(int)teamKind][index].SetUltimateActive(false);
	}

	public void OnChangedChampionBarrierRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		if (_halfChampCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[(int)teamKind][index].SetBarrierRatio(Mathf.Min(ratio, 1f));
	}

	public void OnChampionDead(BattleTeamKind teamKind, int index)
	{
		if (_halfChampCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[(int)teamKind][index].gameObject.SetActive(false);
	}

	public void OnChampionRevival(BattleTeamKind teamKind, int index)
	{
		if (_halfChampCount <= index)
			return;

		int teamIndex = (int)teamKind;

		_champStatusBarContainer[(int)teamKind][index].gameObject.SetActive(true);
	}
}
