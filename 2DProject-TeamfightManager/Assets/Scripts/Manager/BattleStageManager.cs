using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ���������� �����ϴ� �Ŵ��� Ŭ����..
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

		// Red Team ��ü ����..
		team = _teamManager.GetTeamInstance(redTeamName);
		redTeam = team.GetComponent<BattleTeam>();

		// Blue Team ��ü ����..
		team = _teamManager.GetTeamInstance(blueTeamName);
		blueTeam = team.GetComponent<BattleTeam>();

		// �� �� ������Ʈ���� �ʿ��� ������ �Ѱ��ֱ�..
		redTeam.gameManager = gameManager;
        redTeam.battleTeamKind = BattleTeamKind.RedTeam;
		redTeam.enemyTeam = blueTeam;
		redTeam.spawnArea = _redTeamSpawnArea;

        blueTeam.gameManager = gameManager;
        blueTeam.battleTeamKind = BattleTeamKind.BlueTeam;
		blueTeam.enemyTeam = redTeam;
		blueTeam.spawnArea = _blueTeamSpawnArea;

		// �� �� ������Ʈ�� �̺�Ʈ ����..
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

		// ������ ���̺� �ѱ� ���� ���� �� �Ѱ��ֱ�..
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

	// �� ������ �ε����� �޾ƿ� è�Ǿ��� �̸��� �����ϴ� �Լ�..
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

	// ��Ʋ ���� �ð� ���ŵǸ� ȣ��Ǵ� �ݹ� �Լ�..
	private void OnUpdateBattleRemainTime(float remainTime)
	{
		if (remainTime <= 0f)
		{
            _battleStageDataTable.OnUpdateBattleRemainTime -= OnUpdateBattleRemainTime;

            BattleTeamKind winTeam = BattleTeamKind.End;

            if( _battleStageDataTable.redTeamBattleFightData.teamTotalKill != _battleStageDataTable.blueTeamBattleFightData.teamTotalKill )
            {
                winTeam = (_battleStageDataTable.redTeamBattleFightData.teamTotalKill > _battleStageDataTable.blueTeamBattleFightData.teamTotalKill)
                ? BattleTeamKind.RedTeam : BattleTeamKind.BlueTeam;
            }

            if (null != _battleStageDataTable)
            {
				_battleStageDataTable.EndBattle(redTeam, blueTeam, winTeam);
			}

            OnBattleEnd();
            _battleStageDataTable.Reset();

            OnEndBattle?.Invoke(winTeam);

			ExitBattleStage();
        }
	}

	// ��Ʋ ���� �� ȣ��� �Լ�..
	public void OnBattleEnd()
	{
#if UNITY_EDITOR
		Debug.Log("��Ʋ�� ����Ǿ���.");
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

	// ��Ʋ ���������� è�Ǿ���� ��Ʋ ������ �ٲ� ������ ȣ��ȴ�..
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
