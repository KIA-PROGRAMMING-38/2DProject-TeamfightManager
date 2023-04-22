using UnityEngine;
using UnityEngine.SceneManagement;

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
	public SummonObjectManager summonObjectManager { get; private set; }

	public BattleStageManager battleStageManager { get; private set; }
	public BanpickRunner banpickRunner { get; private set; }

	[SerializeField] private GameGlobalData _gameGlobalData;
	public GameGlobalData gameGlobalData { get => _gameGlobalData; }

	private string _curSceneName;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

        GameSaveLoader.SaveGameFile( 0, this );
        LoadFile( 0 );
        SetupManager();
	}

	private void Start()
	{
		InitializeScene();
	}

	// 저장된 파일을 불러오는 메소드..
	private void LoadFile(int loadFileNumber)
	{
        // DataTable Manager 생성..
        GameObject newGameObject = new GameObject( "DataTable Manager" );
        DontDestroyOnLoad( newGameObject );
        newGameObject.transform.parent = transform;
        dataTableManager = newGameObject.AddComponent<DataTableManager>();

        GameSaveLoader.LoadGameFile(loadFileNumber, this);
	}

	private void InitializeScene()
	{
        _curSceneName = SceneManager.GetActiveScene().name;

		switch ( _curSceneName )
		{
			case SceneNameTable.STADIUM:
				CreateBattleStageManager();
				CreateBanpickRunner();
				SceneManager.LoadSceneAsync(SceneNameTable.BANPICK_UI, LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(SceneNameTable.BATTLETEAM_INFO_UI, LoadSceneMode.Additive);
				//SceneManager.LoadScene(SceneNameTable.CHAMP_STATUSBAR_UI, LoadSceneMode.Additive);

				dataTableManager.battleStageDataTable.OnStartBattle -= OnStartBattle;
				dataTableManager.battleStageDataTable.OnStartBattle += OnStartBattle;
				break;
		}
	}

	private void OnStartBattle()
	{
		if( _curSceneName == SceneNameTable.STADIUM)
		{
            SceneManager.LoadSceneAsync( SceneNameTable.CHAMP_STATUSBAR_UI, LoadSceneMode.Additive );
            SceneManager.UnloadSceneAsync( SceneNameTable.BANPICK_UI );
        }

		SceneManager.LoadSceneAsync(SceneNameTable.BATTLESTAGE, LoadSceneMode.Additive);
		battleStageManager.StartBattle();

		battleStageManager.OnEndBattle -= OnEndBattle;
		battleStageManager.OnEndBattle += OnEndBattle;
    }

	private void OnEndBattle(BattleTeamKind winTeam)
	{
        battleStageManager.OnEndBattle -= OnEndBattle;

		if ( _curSceneName == SceneNameTable.STADIUM )
		{
			dataTableManager.statisticsDataTable.AddBattleTeamFightData( dataTableManager.battleStageDataTable.redTeamBattleFightData,
				dataTableManager.battleStageDataTable.blueTeamBattleFightData, winTeam );
		}
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

	private void CreateBanpickRunner()
	{
		GameObject newGameObject = new GameObject("Banpick Runner");
		banpickRunner = newGameObject.AddComponent<BanpickRunner>();

		banpickRunner.gameManager = this;
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
		// 데이터 테이블만 따로 생성한 뒤 파일을 로드하고 나머지 매니저를 생성해야 하므로 여기서는 데이터 테이블 생성 안함..
		// ==========================================================================================
		// Effect Manager 생성..
		newGameObject = new GameObject("Effect Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		effectManager = newGameObject.AddComponent<EffectManager>();

		newGameObject = new GameObject("Projectile Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		summonObjectManager = newGameObject.AddComponent<SummonObjectManager>();

		// Champion Manager 생성..
		newGameObject = new GameObject("Champion Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		championManager = newGameObject.AddComponent<ChampionManager>();

		// Pilot Manager 생성..
		newGameObject = new GameObject("pilot Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		pilotManager = newGameObject.AddComponent<PilotManager>();

		// Team Manager 생성..
		newGameObject = new GameObject("Team Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		teamManager = newGameObject.AddComponent<TeamManager>();

		// 생성했으니 참조 넘겨주기..
		championManager.gameManager = this;
		pilotManager.gameManager = this;
        teamManager.gameManager = this;
        effectManager.gameManager = this;
		summonObjectManager.gameManager = this;
	}
}
