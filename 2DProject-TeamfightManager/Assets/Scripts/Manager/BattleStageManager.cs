using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배틀 스테이지를 관리하는 매니저 클래스..
/// </summary>
public class BattleStageManager : MonoBehaviour
{
	public event Action<BattleTeamKind> OnEndBattle;

	public GameManager gameManager
	{
		private get => _gameManager;
		set
		{
			_gameManager = value;
			_dataTableManager = gameManager.dataTableManager;
			_championManager = gameManager.championManager;
			_pilotManager = gameManager.pilotManager;
			_teamManager = gameManager.teamManager;

            _battleStageDataTable = _dataTableManager.battleStageDataTable;
			_battleStageDataTable.battleStageManager = this;

			int pilotCount = gameManager.gameGlobalData.PilotCount;
			_battleStageDataTable.battleChampionTotalCount = Math.Min(pilotCount, 4) * 2;

			SetupBattleTeam(_dataTableManager.teamDataTable.GetTeamName(0), _dataTableManager.teamDataTable.GetTeamName(1));
		}
	}

	private GameManager _gameManager;
	private ChampionManager _championManager;
	private PilotManager _pilotManager;
	private DataTableManager _dataTableManager;
	private BattleStageDataTable _battleStageDataTable;
	private TeamManager _teamManager;

	public BattleTeam redTeam { get; private set; }
	public BattleTeam blueTeam { get; private set; }

	public Vector2[] _redTeamSpawnArea =
	{
		new Vector2(3f, -2.5f), new Vector2(5f, 1.5f)
	};

	public Vector2[] _blueTeamSpawnArea =
	{
		new Vector2(-5f, -2.5f), new Vector2(-3f, 1.5f)
	};

	private IEnumerator _updateTimerCoroutine;

	private void Awake()
	{
		_updateTimerCoroutine = UpdateBattleTimer();
	}

	private void Start()
	{
		//SetupPilot();

		_battleStageDataTable.OnUpdateBattleRemainTime -= OnUpdateBattleRemainTime;
		_battleStageDataTable.OnUpdateBattleRemainTime += OnUpdateBattleRemainTime;
	}

	public void StartBattle()
	{
		redTeam.StartBattle();
		blueTeam.StartBattle();

		_battleStageDataTable.InitializeBattleTime(gameManager.gameGlobalData.battleFightTime);

		StartCoroutine(_updateTimerCoroutine);
	}

	private IEnumerator UpdateBattleTimer()
	{
		while(true)
		{
			yield return null;
			_battleStageDataTable.updateTime = Time.deltaTime;
		}
	}

	private void SetupBattleTeam(string redTeamName, string blueTeamName)
	{
		Team team = null;

		// Red Team 객체 생성..
		team = _teamManager.GetTeamInstance(redTeamName);
		redTeam = team.GetComponent<BattleTeam>();

		// Blue Team 객체 생성..
		team = _teamManager.GetTeamInstance(blueTeamName);
		blueTeam = team.GetComponent<BattleTeam>();

		// 각 팀 컴포넌트에서 필요한 참조들 넘겨주기..
		redTeam.gameManager = gameManager;
        redTeam.battleTeamKind = BattleTeamKind.RedTeam;
		redTeam.enemyTeam = blueTeam;
		redTeam.spawnArea = _redTeamSpawnArea;

        blueTeam.gameManager = gameManager;
        blueTeam.battleTeamKind = BattleTeamKind.BlueTeam;
		blueTeam.enemyTeam = redTeam;
		blueTeam.spawnArea = _blueTeamSpawnArea;

		// 각 팀 컴포넌트의 이벤트 구독..
		redTeam.OnChangedChampionBattleInfoData -= OnChangedChampionBattleData;
		redTeam.OnChangedChampionBattleInfoData += OnChangedChampionBattleData;

		redTeam.OnChangedChampionHPRatio -= OnUpdateChampionHPRatio;
		redTeam.OnChangedChampionHPRatio += OnUpdateChampionHPRatio;

		redTeam.OnChangedChampionMPRatio -= OnUpdateChampionMPRatio;
		redTeam.OnChangedChampionMPRatio += OnUpdateChampionMPRatio;

		redTeam.OnChampionUseUltimate -= OnChampionUseUltimate;
		redTeam.OnChampionUseUltimate += OnChampionUseUltimate;

		redTeam.OnChangedChampionBarrierRatio -= OnChangedChampionBarrierRatio;
		redTeam.OnChangedChampionBarrierRatio += OnChangedChampionBarrierRatio;


		blueTeam.OnChangedChampionBattleInfoData -= OnChangedChampionBattleData;
		blueTeam.OnChangedChampionBattleInfoData += OnChangedChampionBattleData;

		blueTeam.OnChangedChampionHPRatio -= OnUpdateChampionHPRatio;
		blueTeam.OnChangedChampionHPRatio += OnUpdateChampionHPRatio;

		blueTeam.OnChangedChampionMPRatio -= OnUpdateChampionMPRatio;
		blueTeam.OnChangedChampionMPRatio += OnUpdateChampionMPRatio;

		blueTeam.OnChampionUseUltimate -= OnChampionUseUltimate;
		blueTeam.OnChampionUseUltimate += OnChampionUseUltimate;

		blueTeam.OnChangedChampionBarrierRatio -= OnChangedChampionBarrierRatio;
		blueTeam.OnChangedChampionBarrierRatio += OnChangedChampionBarrierRatio;

		// 데이터 테이블에 넘길 정보 생성 및 넘겨주기..
		List<BattlePilotFightData> redTeamBattlePilotFightDatas = redTeam.battlePilotFightData;
		List<BattlePilotFightData> blueTeamBattlePilotFightDatas = blueTeam.battlePilotFightData;

		_battleStageDataTable.Initialize(redTeam.teamName, redTeamBattlePilotFightDatas,
			blueTeam.teamName, blueTeamBattlePilotFightDatas);
	}

	public void PickChampion(BattleTeamKind teamKind, int index, string champName)
	{
		switch (teamKind)
		{
			case BattleTeamKind.RedTeam:
				redTeam.AddChampion(index, champName);
				_battleStageDataTable.redTeamBattleFightData.pilotFightDataContainer[index].championName = champName;

                break;
			case BattleTeamKind.BlueTeam:
				blueTeam.AddChampion(index, champName);
				_battleStageDataTable.blueTeamBattleFightData.pilotFightDataContainer[index].championName = champName;

				break;
		}
	}

	// 팀 종류와 인덱스를 받아와 챔피언의 이름을 리턴하는 함수..
	public string GetChampionName(BattleTeamKind teamKind, int index)
	{
		return GetChampion(teamKind, index).data.name;
	}

	public Champion GetChampion(BattleTeamKind teamKind, int index)
	{
		if (teamKind == BattleTeamKind.RedTeam)
			return redTeam.GetChampion(index);
		else
			return blueTeam.GetChampion(index);
	}

	public Pilot GetPilot(BattleTeamKind teamKind, int index)
	{
		if (teamKind == BattleTeamKind.RedTeam)
			return redTeam.GetPilot(index);
		else
			return blueTeam.GetPilot(index);
	}

	// 배틀 남은 시간 갱신되면 호출되는 콜백 함수..
	private void OnUpdateBattleRemainTime(float remainTime)
	{
		if (remainTime <= 0f)
		{
            if ( _battleStageDataTable.redTeamBattleFightData.teamTotalKill == _battleStageDataTable.blueTeamBattleFightData.teamTotalKill )
            {

            }
            else
            {
                OnBattleEnd();
                _battleStageDataTable.Reset();

                BattleTeamKind winTeam = (_battleStageDataTable.redTeamBattleFightData.teamTotalKill > _battleStageDataTable.blueTeamBattleFightData.teamTotalKill)
                ? BattleTeamKind.RedTeam : BattleTeamKind.BlueTeam;
                OnEndBattle?.Invoke( winTeam );
            }
		}
	}

	// 배틀 종료 시 호출될 함수..
	public void OnBattleEnd()
	{
#if UNITY_EDITOR
		Debug.Log("배틀이 종료되었다.");
#endif

		StopAllCoroutines();

		redTeam.OnBattleEnd();
		blueTeam.OnBattleEnd();
    }

	public void ExitBattleStage()
	{
		redTeam.ExitBattleStage();
		blueTeam.ExitBattleStage();
	}

	// 배틀 스테이지의 챔피언들의 배틀 정보가 바뀔 때마다 호출된다..
	private void OnChangedChampionBattleData(BattleTeamKind teamKind, int index, BattleInfoData data)
	{
		_battleStageDataTable?.ModifyChampionBattleData(teamKind, index, data);
	}

	private void OnUpdateChampionHPRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		_battleStageDataTable?.ModifyChampionHPRatio(teamKind, index, ratio);
	}

	private void OnUpdateChampionMPRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		_battleStageDataTable?.ModifyChampionMPRatio(teamKind, index, ratio);
	}

	private void OnChampionUseUltimate(BattleTeamKind teamKind, int index)
	{
		_battleStageDataTable?.ModifyChampionUseUltimateState(teamKind, index);
	}

	private void OnChangedChampionBarrierRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		_battleStageDataTable?.ModifyChampionBarrierRatio(teamKind, index, ratio);
	}

	public void OnChampionDeadState(BattleTeamKind teamKind, int index)
	{
		_battleStageDataTable?.OnChampionDeath(teamKind, index);
	}

	public void OnChampionRevivalState(BattleTeamKind teamKind, int index)
	{
		_battleStageDataTable?.OnChampionRevival(teamKind, index);
	}
}
