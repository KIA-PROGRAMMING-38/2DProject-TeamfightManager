using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageDataTable
{
	public class BanpickStageInfo
	{
		public string champName;
		public BanpickStageKind stageKind;
		public BattleTeamKind teamKind;
		public int level;

		public void Set(BanpickStageKind stageKind, BattleTeamKind teamKind, int level)
		{
			this.stageKind = stageKind;
			this.teamKind = teamKind;
			this.level = level;
		}
	}
		

	public ChampionDataTable championDataTable { private get; set; }
	public BattleStageManager battleStageManager { private get; set; }

	public event Action<float> OnUpdateBattleRemainTime;
	private float _battleRemainTime = 0f;   // 배틀 총 남은 시간..

	// 밴픽 관련 이벤트 함수..
	public event Action OnStartBattle;
	public event Action<string> OnClickedSelectChampionButton;
	public event Action OnBanpickEnd;
	public event Action<string, BanpickStageKind, BattleTeamKind, int> OnBanpickUpdate;
	public event Action<BanpickStageKind> OnBanpickOneStageStart;

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

	public BanpickStageInfo curBanpickStageInfo { get; set; }
	public BattleTeamFightData redTeamBattleFightData { get; private set; }
	public BattleTeamFightData blueTeamBattleFightData { get; private set; }
	public int maxBanpickLevel { get; set; }
	public int totalBanChampCount { get; set; }
	public int curBanpickLevel { get; private set; }

	public BattleStageDataTable()
	{
		curBanpickStageInfo = new BanpickStageInfo();
		redTeamBattleFightData = new BattleTeamFightData();
		blueTeamBattleFightData = new BattleTeamFightData();

		curBanpickLevel = 1;
	}

	// 배틀 시작 시 총 배틀해야하는 시간 받아서 초기화하는 부분..
	public void Initialize(float gameBattleTime, string redTeamName, List<BattlePilotFightData> redTeamBattlePilotFightDatas
		, string blueTeamName, List<BattlePilotFightData> blueTeamBattlePilotFightDatas)
	{
		_battleRemainTime = gameBattleTime;

		redTeamBattleFightData.teamName = redTeamName;
		blueTeamBattleFightData.teamName = blueTeamName;

		redTeamBattleFightData.pilotFightDataContainer = redTeamBattlePilotFightDatas;
		blueTeamBattleFightData.pilotFightDataContainer = blueTeamBattlePilotFightDatas;
	}

	// 배틀 끝났을 때 관련 데이터 처리하는 부분..
	public void Reset()
	{
		OnUpdateBattleRemainTime = null;

		banpickChampContainer.Clear();

		curBanpickLevel = 1;
		maxBanpickLevel = 0;
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
		curBanpickStageInfo.champName = championName;
		OnBanpickUpdate?.Invoke(championName, stageKind, teamKind, index);

		banpickChampContainer.Add(championName, stageKind);

		if (stageKind == BanpickStageKind.Ban)
		{
			if (BattleTeamKind.RedTeam == teamKind)
			{
				redTeamBattleFightData.banChampionContainer.Add(championName);
			}
			else if (BattleTeamKind.BlueTeam == teamKind)
			{
				blueTeamBattleFightData.banChampionContainer.Add(championName);
			}
		}

		++curBanpickLevel;
    }

	public void StartBattle()
	{
		OnStartBattle?.Invoke();
	}
	
	public void StartBanpick(BanpickStageKind stageKind, BattleTeamKind teamKind)
	{
		curBanpickStageInfo.Set(stageKind, teamKind, 0);
	}

	public void StartBanpickOneStage(BanpickStageKind curStageKind)
	{
		OnBanpickOneStageStart?.Invoke(curStageKind);
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
		if (BattleTeamKind.RedTeam == teamKind)
		{
			++blueTeamBattleFightData.teamTotalKill;
		}
		else if (BattleTeamKind.BlueTeam == teamKind)
		{
			++redTeamBattleFightData.teamTotalKill;
		}

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