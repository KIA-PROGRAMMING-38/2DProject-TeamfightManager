using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
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

		// Red Team ��ü ����..
		newGameobject = new GameObject("Red Team");
		newGameobject.transform.parent = transform;
		redTeam = newGameobject.AddComponent<BattleTeam>();

		// Blue Team ��ü ����..
		newGameobject = new GameObject("Blue Team");
		newGameobject.transform.parent = transform;
		blueTeam = newGameobject.AddComponent<BattleTeam>();

		// �� �� ������Ʈ���� �ʿ��� ������ �Ѱ��ֱ�..
		redTeam.enemyTeam = blueTeam;
		redTeam.championManager = _championManager;
		redTeam.pilotManager = _pilotManager;
		redTeam.battleStageManager = this;
		redTeam.spawnArea = _redTeamSpawnArea;

		blueTeam.enemyTeam = redTeam;
		blueTeam.championManager = _championManager;
		blueTeam.pilotManager = _pilotManager;
		blueTeam.battleStageManager = this;
		blueTeam.spawnArea = _blueTeamSpawnArea;
	}

	// ��Ʋ ���������� �� ������ ���Ϸ� �������ִ� �Լ�..
	private void SetupPilot()
	{
		int pilotCount = 10;

		for (int i = 0; i < pilotCount; ++i)
		{
			redTeam.AddPilot("Faker", "Swordman");
			blueTeam.AddPilot("Faker", "Swordman");
		}

		redTeam.TestColorChange(Color.red);
		blueTeam.TestColorChange(Color.blue);
	}

	UnityEngine.UI.Text _remainTimeText;

	private void OnUpdateBattleRemainTime(float remainTime)
	{
		if(null == _remainTimeText)
		{
			_remainTimeText = GameObject.Find("remainingTimeText").GetComponent<UnityEngine.UI.Text>();
		}

		// �Ҽ��� �ڸ� �ø� �� �ؽ�Ʈ ǥ��..
		_remainTimeText.text = ((int)(remainTime + 0.99f)).ToString();

		if (remainTime <= 0f)
		{
			OnBattleEnd();

			_battleStageDataTable.Reset();
		}
	}

	// ��Ʋ ���� �� ȣ��� �Լ�..
	private void OnBattleEnd()
	{
		Debug.Log("��Ʋ�� ����Ǿ���.");

		StopAllCoroutines();

		redTeam.OnBattleEnd();
		blueTeam.OnBattleEnd();
	}
}
