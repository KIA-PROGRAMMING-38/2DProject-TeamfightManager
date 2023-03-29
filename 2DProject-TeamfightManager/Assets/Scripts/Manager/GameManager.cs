using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public ChampionManager championManager { get; private set; }
	public TeamManager teamManager { get; private set; }
	public DataTableManager dataTableManager { get; private set; }
	public PilotManager pilotManager { get; private set; }

	private GameSaveLoader _gameSaveLoader;

	public BattleStageManager battleStageManager;

	private void Awake()
	{
		SetupManager();

		LoadFile("");

		CreateBattleStageManager();
	}

	private void LoadFile(string fileName)
	{
		_gameSaveLoader.LoadGameFile(fileName);
	}

	private void CreateBattleStageManager()
	{
		// BattleStage Manager 생성..
		GameObject newGameObject = new GameObject("BattleStage Manager");
		newGameObject.transform.parent = transform;
		battleStageManager = newGameObject.AddComponent<BattleStageManager>();

		battleStageManager.gameManager = this;
	}

	private void SetupManager()
	{
		// 생성 순서 바꿔도 상관없게 일단 빈 객체 생성..
		GameObject newGameObject = null;

		// ==========================================================================================
		// --- 이벤트 함수 호출 생각하면서 생성 순서 정하기..
		// 1순위 : 데이터테이블 or saveLoader, 나머진 상관없을 것 같은 느낌??
		// ==========================================================================================
		// SaveLoader 생성..
		_gameSaveLoader = new GameSaveLoader();

		// DataTable Manager 생성..
		newGameObject = new GameObject("DataTable Manager");
		newGameObject.transform.parent = transform;
		dataTableManager = newGameObject.AddComponent<DataTableManager>();

		// Champion Manager 생성..
		newGameObject = new GameObject("Champion Manager");
		newGameObject.transform.parent = transform;
		championManager = newGameObject.AddComponent<ChampionManager>();

		// Pilot Manager 생성..
		newGameObject = new GameObject("pilot Manager");
		newGameObject.transform.parent = transform;
		pilotManager = newGameObject.AddComponent<PilotManager>();

		// Team Manager 생성..
		newGameObject = new GameObject("Team Manager");
		newGameObject.transform.parent = transform;
		teamManager = newGameObject.AddComponent<TeamManager>();

		// 생성했으니 참조 넘겨주기..
		_gameSaveLoader.gameManager = this;
		championManager.gameManager = this;
		teamManager.gameManager = this;
		pilotManager.gameManager = this;
	}
}
