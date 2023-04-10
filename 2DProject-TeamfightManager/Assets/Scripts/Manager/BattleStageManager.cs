using System;
using UnityEngine;

public enum BattleTeamKind
{
	RedTeam,
	BlueTeam
}

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
		
	}

	private void Start()
	{
		SetupBattleTeam();
		SetupPilot();

		_battleStageDataTable.OnUpdateBattleRemainTime -= OnUpdateBattleRemainTime;
		_battleStageDataTable.OnUpdateBattleRemainTime += OnUpdateBattleRemainTime;

		_battleStageDataTable.Initialize(10f);
	}

	private void Update()
	{
		_battleStageDataTable.updateTime = Time.deltaTime;
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
		redTeam.OnChangedChampionBattleInfoData -= OnChangedMyChampionBattleData;
		redTeam.OnChangedChampionBattleInfoData += OnChangedMyChampionBattleData;

		redTeam.OnChangedChampionBattleInfoData -= OnChangedMyChampionBattleData;
		blueTeam.OnChangedChampionBattleInfoData += OnChangedMyChampionBattleData;
	}

	// 배틀 스테이지의 각 팀들의 파일럿 생성해주는 함수..
	private void SetupPilot()
	{
		int pilotCount = 4;

		for (int i = 0; i < pilotCount; ++i)
		{
			redTeam.AddPilot("Faker", "Swordman");
			blueTeam.AddPilot("Faker", "Swordman");
		}

		redTeam.TestColorChange(Color.red);
		blueTeam.TestColorChange(Color.blue);
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

	private void OnChangedMyChampionBattleData(BattleTeamKind teamKind, int index, BattleInfoData data)
	{
		Debug.Log($"정보가 변경되었다. 팀 : {teamKind.ToString()}, 인덱스 : {index}");
		_battleStageDataTable?.ModifyChampionBattleData(teamKind, index, data);
	}
}
