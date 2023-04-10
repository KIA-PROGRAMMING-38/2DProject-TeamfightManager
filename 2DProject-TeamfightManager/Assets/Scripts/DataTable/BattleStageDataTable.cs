using System;
using System.Collections.Generic;

public class BattleStageDataTable
{
	public event Action<float> OnUpdateBattleRemainTime;
	private float _battleRemainTime = 0f;   // 배틀 총 남은 시간..

	private Action<int, BattleInfoData>[] OnChangedChampionBattleData;

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

	public BattleStageDataTable()
	{
		OnChangedChampionBattleData = new Action<int, BattleInfoData>[2];
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

	public void ModifyChampionBattleData(BattleTeamKind teamKind, int index, BattleInfoData data)
	{
		OnChangedChampionBattleData[(int)teamKind]?.Invoke(index, data);
	}

	public void AddChampionBattleDataEvent(BattleTeamKind teamKind, Action<int, BattleInfoData> callbackFunc)
	{
		OnChangedChampionBattleData[(int)teamKind] -= callbackFunc;
		OnChangedChampionBattleData[(int)teamKind] += callbackFunc;
	}
}