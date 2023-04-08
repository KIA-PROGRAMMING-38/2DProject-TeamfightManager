using UnityEngine;

/// <summary>
/// 모든 게임에 관련된 것들을 관리하기 위한 클래스..
/// </summary>
public class GameManager : MonoBehaviour
{
	public ChampionManager championManager { get; private set; }
	public TeamManager teamManager { get; private set; }
	public DataTableManager dataTableManager { get; private set; }
	public PilotManager pilotManager { get; private set; }
	public EffectManager effectManager { get; private set; }
	public UIManager uiManager { get; private set; }

	public BattleStageManager battleStageManager;

	private void Awake()
	{
		SetupManager();

		LoadFile(0);
	}

	private void Start()
	{
		CreateBattleStageManager();
	}

	// 저장된 파일을 불러오는 메소드..
	private void LoadFile(int loadFileNumber)
	{
		GameSaveLoader.LoadGameFile(loadFileNumber, dataTableManager);
	}

	// 배틀 스테이지를 생성하는 함수..
	private void CreateBattleStageManager()
	{
		// BattleStage Manager 생성..
		GameObject newGameObject = new GameObject("BattleStage Manager");
		newGameObject.transform.parent = transform;
		battleStageManager = newGameObject.AddComponent<BattleStageManager>();

		battleStageManager.gameManager = this;
	}

	/// <summary>
	/// 매니저 및 GameSaveLoader 생성 및 참조 연결..
	/// </summary>
	private void SetupManager()
	{
		// 생성 순서 바꿔도 상관없게 일단 빈 객체 생성..
		GameObject newGameObject = null;

		// ==========================================================================================
		// --- 이벤트 함수 호출 생각하면서 생성 순서 정하기..
		// 1순위 : 데이터테이블, 나머진 상관없을 것 같은 느낌??
		// ==========================================================================================
		// DataTable Manager 생성..
		newGameObject = new GameObject("DataTable Manager");
		newGameObject.transform.parent = transform;
		dataTableManager = newGameObject.AddComponent<DataTableManager>();

		// Effect Manager 생성..
		newGameObject = new GameObject("Effect Manager");
		newGameObject.transform.parent = transform;
		effectManager = newGameObject.AddComponent<EffectManager>();

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

		// UI Manager 생성..
		newGameObject = new GameObject("UI Manager");
		newGameObject.transform.parent = transform;
		uiManager = newGameObject.AddComponent<UIManager>();

		// 생성했으니 참조 넘겨주기..
		championManager.gameManager = this;
		teamManager.gameManager = this;
		pilotManager.gameManager = this;
		effectManager.gameManager = this;
		uiManager.gameManager = this;
	}
}
