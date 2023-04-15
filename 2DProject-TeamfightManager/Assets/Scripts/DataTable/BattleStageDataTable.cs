using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageDataTable
{
	public ChampionDataTable championDataTable { private get; set; }
	public BattleStageManager battleStageManager { private get; set; }

	public event Action<float> OnUpdateBattleRemainTime;
	private float _battleRemainTime = 0f;   // 배틀 총 남은 시간..

	// 밴픽 관련 이벤트 함수..
	public event Action<string> OnClickedSelectChampionButton;
	public event Action OnBanpickEnd;
	public event Action<string, BanpickStageKind, BattleTeamKind, int> OnBanpickUpdate;

	// 챔피언 데이터가 변할 때 호출될 이벤트 함수..
	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleData;

	public event Action<BattleTeamKind, int, float> OnChangedChampionHPRatio;
	public event Action<BattleTeamKind, int, float> OnChangedChampionMPRatio;
	public event Action<BattleTeamKind, int> OnChampionUseUltimate;
	public event Action<BattleTeamKind, int> OnChampionDeadEvent;
	public event Action<BattleTeamKind, int> OnChampionRevivalEvent;
	public event Action<BattleTeamKind, int, float> OnChangedChampionBarrierRatio;

	private Dictionary<string, BanpickStageKind> banpickChampContainer = new Dictionary<string, BanpickStageKind>();

	public int battleChampionTotalCount { get; set; }

	// 배틀 시간 갱신..
	public float updateTime
	{
		set
		{
			_battleRemainTime -= value;

			_battleRemainTime = MathF.Max(0f, _battleRemainTime);
			OnUpdateBattleRemainTime?.Invoke(_battleRemainTime);
		}
	}

	// 배틀 시작 시 총 배틀해야하는 시간 받아서 초기화하는 부분..
	public void Initialize(float gameBattleTime)
	{
		_battleRemainTime = gameBattleTime;
	}

	// 배틀 끝났을 때 관련 데이터 처리하는 부분..
	public void Reset()
	{
		OnUpdateBattleRemainTime = null;

		banpickChampContainer.Clear();
	}

	// ============================================================================================================
	// --- 밴픽 관련 함수들..
	// ============================================================================================================
	public void EndBanpick()
	{
		OnBanpickEnd?.Invoke();
	}

	public void OnClickedSelectChampButton(string championName)
	{
		if (banpickChampContainer.ContainsKey(championName))
			return;

		OnClickedSelectChampionButton?.Invoke(championName);
	}

	public void UpdateBanpickData(string championName, BanpickStageKind stageKind, BattleTeamKind teamKind, int index)
	{
		OnBanpickUpdate?.Invoke(championName, stageKind, teamKind, index);
	}

	// ============================================================================================================
	// --- 챔피언 정보가 바뀔 때마다 호출되는 콜백 함수들..
	// ============================================================================================================
	public void ModifyChampionBattleData(BattleTeamKind teamKind, int index, BattleInfoData data)
	{
		OnChangedChampionBattleData?.Invoke(teamKind, index, data);
	}

	public void ModifyChampionHPRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		OnChangedChampionHPRatio?.Invoke(teamKind, index, ratio);
	}

	public void ModifyChampionMPRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		OnChangedChampionMPRatio?.Invoke(teamKind, index, ratio);
	}

	public void ModifyChampionUseUltimateState(BattleTeamKind teamKind, int index)
	{
		OnChampionUseUltimate?.Invoke(teamKind, index);
	}

	public void ModifyChampionBarrierRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		OnChangedChampionBarrierRatio?.Invoke(teamKind, index, ratio);
	}

	public void OnChampionDeath(BattleTeamKind teamKind, int index)
	{
		OnChampionDeadEvent?.Invoke(teamKind, index);
	}

	public void OnChampionRevival(BattleTeamKind teamKind, int index)
	{
		OnChampionRevivalEvent?.Invoke(teamKind, index);
	}

	// ============================================================================================================
	// --- 챔피언 및 파일럿 관련된 인스턴스 혹은 리소스를 반환하는 함수들..
	// ============================================================================================================
	public Sprite GetChampionUltimateIconSprite(BattleTeamKind teamKind, int index)
	{
		string championName = battleStageManager.GetChampionName(teamKind, index);

		return championDataTable.GetUltimateIconImage(championName);
	}

	public Transform GetChampionTransform(BattleTeamKind teamKind, int index)
	{
		return battleStageManager.GetChampion(teamKind, index).transform;
	}

	public Pilot GetPilot(BattleTeamKind teamKind, int index)
	{
		return battleStageManager.GetPilot(teamKind, index);
	}
}