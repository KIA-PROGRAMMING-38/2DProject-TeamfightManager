using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageDataTable
{
	public ChampionDataTable championDataTable { private get; set; }
	public BattleStageManager battleStageManager { private get; set; }

	public event Action<float> OnUpdateBattleRemainTime;
	private float _battleRemainTime = 0f;   // 배틀 총 남은 시간..

	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleData;

	public event Action<BattleTeamKind, int, float> OnChangedChampionHPRatio;
	public event Action<BattleTeamKind, int, float> OnChangedChampionMPRatio;
	public event Action<BattleTeamKind, int> OnChampionUseUltimate;
	public event Action<BattleTeamKind, int> OnChampionDeadEvent;
	public event Action<BattleTeamKind, int> OnChampionRevivalEvent;
	public event Action<BattleTeamKind, int, float> OnChangedChampionBarrierRatio;

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

	public Champion GetChampion(BattleTeamKind teamKind, int index)
	{
		return battleStageManager.GetChampion(teamKind, index);
	}

	public Pilot GetPilot(BattleTeamKind teamKind, int index)
	{
		return battleStageManager.GetPilot(teamKind, index);
	}
}