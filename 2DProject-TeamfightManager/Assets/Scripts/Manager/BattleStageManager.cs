using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배틀 스테이지를 관리하는 매니저 클래스..
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

			_battleStageDataTable = _dataTableManager.battleStageDataTable;
			_battleStageDataTable.battleStageManager = this;
		}
	}

	private GameManager _gameManager;
	private ChampionManager _championManager;
	private PilotManager _pilotManager;
	private DataTableManager _dataTableManager;
	private BattleStageDataTable _battleStageDataTable;

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

	private void Awake()
	{
		int pilotCount = gameManager.gameGlobalData.PilotCount;
		_battleStageDataTable.battleChampionTotalCount = Math.Min(pilotCount, 4) * 2;

		SetupBattleTeam();
	}

	private void Start()
	{
		SetupPilot();

		_battleStageDataTable.OnUpdateBattleRemainTime -= OnUpdateBattleRemainTime;
		_battleStageDataTable.OnUpdateBattleRemainTime += OnUpdateBattleRemainTime;
	}

	private void Update()
	{
		_battleStageDataTable.updateTime = Time.deltaTime;
	}

	public void StartBattle()
	{
		_battleStageDataTable.Initialize(gameManager.gameGlobalData.battleFightTime);

		redTeam.StartBattle();
		blueTeam.StartBattle();
	}

	private void SetupBattleTeam()
	{
		GameObject newGameobject = null;

		// Red Team 객체 생성..
		newGameobject = new GameObject("Red Team");
		newGameobject.transform.parent = transform;
		redTeam = newGameobject.AddComponent<BattleTeam>();

		// Blue Team 객체 생성..
		newGameobject = new GameObject("Blue Team");
		newGameobject.transform.parent = transform;
		blueTeam = newGameobject.AddComponent<BattleTeam>();

		// 각 팀 컴포넌트에서 필요한 참조들 넘겨주기..
		redTeam.battleTeamKind = BattleTeamKind.RedTeam;
		redTeam.enemyTeam = blueTeam;
		redTeam.championManager = _championManager;
		redTeam.pilotManager = _pilotManager;
		redTeam.battleStageManager = this;
		redTeam.spawnArea = _redTeamSpawnArea;

		blueTeam.battleTeamKind = BattleTeamKind.BlueTeam;
		blueTeam.enemyTeam = redTeam;
		blueTeam.championManager = _championManager;
		blueTeam.pilotManager = _pilotManager;
		blueTeam.battleStageManager = this;
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
	}

	public void AddPilot(int index, string pilotName)
	{
		int pilotCount = gameManager.gameGlobalData.PilotCount;

		BattleTeamKind teamKind = (BattleTeamKind)(index / pilotCount);
		index %= pilotCount;

		switch (teamKind)
		{
			case BattleTeamKind.RedTeam:
				redTeam.AddPilot(index, pilotName);
				break;
			case BattleTeamKind.BlueTeam:
				blueTeam.AddPilot(index, pilotName);
				break;
		}
	}

	public void PickChampion(BattleTeamKind teamKind, int index, string champName)
	{
		int pilotCount = gameManager.gameGlobalData.PilotCount;

		switch (teamKind)
		{
			case BattleTeamKind.RedTeam:
				redTeam.AddChampion(index, champName);
				break;
			case BattleTeamKind.BlueTeam:
				blueTeam.AddChampion(index, champName);
				break;
		}
	}

	// 배틀 스테이지의 각 팀들의 파일럿 생성해주는 함수..
	private void SetupPilot()
	{
		int pilotCount = gameManager.gameGlobalData.PilotCount;

		List<string> blueTeamPilotCreateOrder = gameManager.gameGlobalData.testBluePilotCreateOrder;
		List<string> blueTeamChampCreateOrder = gameManager.gameGlobalData.testBlueChampionCreateOrder;
		List<string> redTeamPilotCreateOrder = gameManager.gameGlobalData.testRedPilotCreateOrder;
		List<string> redTeamChampCreateOrder = gameManager.gameGlobalData.testRedChampionCreateOrder;

		for (int i = 0; i < pilotCount; ++i)
		{
			//redTeam.AddPilot(redTeamPilotCreateOrder[i % redTeamPilotCreateOrder.Count], redTeamChampCreateOrder[i % redTeamChampCreateOrder.Count]);
			//blueTeam.AddPilot(blueTeamPilotCreateOrder[i % blueTeamPilotCreateOrder.Count], blueTeamChampCreateOrder[i % blueTeamChampCreateOrder.Count]);

			redTeam.AddPilot(i, redTeamPilotCreateOrder[i % redTeamPilotCreateOrder.Count]);
			blueTeam.AddPilot(i, blueTeamPilotCreateOrder[i % blueTeamPilotCreateOrder.Count]);

			redTeam.AddChampion(i, redTeamChampCreateOrder[i % redTeamChampCreateOrder.Count]);
			blueTeam.AddChampion(i, blueTeamChampCreateOrder[i % blueTeamChampCreateOrder.Count]);
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
		// 이 부분은 테스트용 코드다(UI 작업 시 UI 스크립트에서 처리하도록 바꿔야 한다)
		UnityEngine.UI.Text _remainTimeText = GameObject.Find("remainingTimeText").GetComponent<UnityEngine.UI.Text>();

		// 소수점 자리 올린 뒤 텍스트 표현..
		_remainTimeText.text = ((int)(remainTime + 0.99f)).ToString();

		if (remainTime <= 0f)
		{
			OnBattleEnd();

			_battleStageDataTable.Reset();
		}
	}

	// 배틀 종료 시 호출될 함수..
	private void OnBattleEnd()
	{
#if UNITY_EDITOR
		Debug.Log("배틀이 종료되었다.");
#endif

		StopAllCoroutines();

		redTeam.OnBattleEnd();
		blueTeam.OnBattleEnd();
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
