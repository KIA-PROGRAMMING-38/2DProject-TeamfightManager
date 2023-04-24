using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ��� ���ӿ� ���õ� �͵��� �����ϱ� ���� Ŭ����..
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

        LoadFile( 0 );
        SetupManager();
	}

	private void Start()
	{
		InitializeScene();
	}

	// ����� ������ �ҷ����� �޼ҵ�..
	private void LoadFile(int loadFileNumber)
	{
        // DataTable Manager ����..
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
			case SceneNameTable.DORMITORY:
				SceneManager.LoadScene(SceneNameTable.DORMITORY_UI, LoadSceneMode.Additive);

				break;

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

	// ��Ʋ ���������� �����ϴ� �Լ�..
	private void CreateBattleStageManager()
	{
		// BattleStage Manager ����..
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
	/// �Ŵ��� �� GameSaveLoader ���� �� ���� ����..
	/// </summary>
	private void SetupManager()
	{
		// ���� ���� �ٲ㵵 ������� �ϴ� �� ��ü ����..
		GameObject newGameObject = null;

		// ==========================================================================================
		// --- �̺�Ʈ �Լ� ȣ�� �����ϸ鼭 ���� ���� ���ϱ�..
		// 1���� : ���������̺�, ������ ������� �� ���� ����??
		// ������ ���̺� ���� ������ �� ������ �ε��ϰ� ������ �Ŵ����� �����ؾ� �ϹǷ� ���⼭�� ������ ���̺� ���� ����..
		// ==========================================================================================
		// Effect Manager ����..
		newGameObject = new GameObject("Effect Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		effectManager = newGameObject.AddComponent<EffectManager>();

		newGameObject = new GameObject("Projectile Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		summonObjectManager = newGameObject.AddComponent<SummonObjectManager>();

		// Champion Manager ����..
		newGameObject = new GameObject("Champion Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		championManager = newGameObject.AddComponent<ChampionManager>();

		// Pilot Manager ����..
		newGameObject = new GameObject("pilot Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		pilotManager = newGameObject.AddComponent<PilotManager>();

		// Team Manager ����..
		newGameObject = new GameObject("Team Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		teamManager = newGameObject.AddComponent<TeamManager>();

		// ���������� ���� �Ѱ��ֱ�..
		championManager.gameManager = this;
		pilotManager.gameManager = this;
        teamManager.gameManager = this;
        effectManager.gameManager = this;
		summonObjectManager.gameManager = this;
	}
}
