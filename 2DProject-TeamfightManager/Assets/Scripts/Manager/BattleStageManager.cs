using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

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
		}
	}

	private GameManager _gameManager;
	private ChampionManager _championManager;
	private PilotManager _pilotManager;
	private DataTableManager _dataTableManager;

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

	private void Start()
	{
		SetupBattleTeam();
		SetupPilot();
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

	private void SetupPilot()
	{
		int pilotCount = 1;

		for (int i = 0; i < pilotCount; ++i)
		{
			redTeam.AddPilot("Test", "Swordman");
			blueTeam.AddPilot("Test", "Swordman");
		}

		redTeam.TestColorChange(Color.red);
		blueTeam.TestColorChange(Color.blue);
	}

	public void OnChampionDead(BattleTeam battleTeam, Champion champion)
	{
		StartCoroutine(Test(battleTeam, champion));
	}

	IEnumerator Test( BattleTeam battleTeam, Champion champion )
	{
		yield return new WaitForSeconds(1f);

		battleTeam.OnSuccessRevival(champion);
	}
}
