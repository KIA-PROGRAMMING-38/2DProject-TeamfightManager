using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ���������� �����ϴ� �Ŵ��� Ŭ����..
/// </summary>
public class BattleStageManager : MonoBehaviour
{
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

			SetupBattleTeam( _gameManager.gameGlobalData.DefaultRedTeamName, _gameManager.gameGlobalData.DefaultBlueTeamName );
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

		_battleStageDataTable.Initialize(gameManager.gameGlobalData.battleFightTime, redTeam.teamName, redTeamBattlePilotFightDatas,
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

	// ��Ʋ ���������� �� ������ ���Ϸ� �������ִ� �Լ�..
	private void SetupPilot()
	{
		int pilotCount = gameManager.gameGlobalData.PilotCount;

		List<string> blueTeamPilotCreateOrder = gameManager.gameGlobalData.testBluePilotCreateOrder;
		List<string> blueTeamChampCreateOrder = gameManager.gameGlobalData.testBlueChampionCreateOrder;
		List<string> redTeamPilotCreateOrder = gameManager.gameGlobalData.testRedPilotCreateOrder;
		List<string> redTeamChampCreateOrder = gameManager.gameGlobalData.testRedChampionCreateOrder;

		for (int i = 0; i < pilotCount; ++i)
		{
			//redTeam.AddChampion(i, redTeamChampCreateOrder[i % redTeamChampCreateOrder.Count]);
			//blueTeam.AddChampion(i, blueTeamChampCreateOrder[i % blueTeamChampCreateOrder.Count]);
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
			OnBattleEnd();

			_battleStageDataTable.Reset();
		}
	}

	// ��Ʋ ���� �� ȣ��� �Լ�..
	private void OnBattleEnd()
	{
#if UNITY_EDITOR
		Debug.Log("��Ʋ�� ����Ǿ���.");
#endif

		StopAllCoroutines();

		redTeam.OnBattleEnd();
		blueTeam.OnBattleEnd();
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
