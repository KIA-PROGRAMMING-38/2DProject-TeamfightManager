using System;
using System.Collections.Generic;

public class BattleStageDataTable
{
	public event Action<float> OnUpdateBattleRemainTime;
	private float _battleRemainTime = 0f;   // 배틀 총 남은 시간..

	private Dictionary<Champion, BattleStageChampionDataTable> battleStageChampDataTables = new Dictionary<Champion, BattleStageChampionDataTable>();
	public int battleStageChampDataTableCount { get => battleStageChampDataTables.Count; }

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

	public void AddPilot(Pilot pilot)
	{
		BattleStageChampionDataTable battleStagePilotDataTable = new BattleStageChampionDataTable();
		battleStagePilotDataTable.pilot = pilot;
		battleStagePilotDataTable.champion = pilot.battleComponent.controlChampion;

		// 챔피언의 이벤트 등록..
		battleStagePilotDataTable.champion.OnHit -= OnChampionHit;
		battleStagePilotDataTable.champion.OnHit += OnChampionHit;

		battleStagePilotDataTable.champion.OnHill -= OnChampionHill;
		battleStagePilotDataTable.champion.OnHill += OnChampionHill;

		battleStageChampDataTables.Add(battleStagePilotDataTable.champion, battleStagePilotDataTable);
	}

	private void OnChampionHit(Champion sufferhampion, Champion hitChampion, int damage)
	{
		battleStageChampDataTables[sufferhampion].takeDamage = damage;
		battleStageChampDataTables[hitChampion].attackDamage = damage;
	}

	private void OnChampionHill(Champion sufferhampion, Champion hillChampion, int hill)
	{
		battleStageChampDataTables[hillChampion].attackDamage = hill;
	}
}