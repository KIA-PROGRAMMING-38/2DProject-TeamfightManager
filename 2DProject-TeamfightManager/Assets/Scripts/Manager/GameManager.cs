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

	public BattleStageManager battleStageManager { get; private set; }
	public BanpickRunner banpickRunner { get; private set; }

	[SerializeField] private GameGlobalData _gameGlobalData;
	public GameGlobalData gameGlobalData { get => _gameGlobalData; }

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

		SetupManager();

		GameSaveLoader.SaveGameFile(0, this);

		LoadFile(0);
	}

	private void Start()
	{
		InitializeScene();
	}

	// ����� ������ �ҷ����� �޼ҵ�..
	private void LoadFile(int loadFileNumber)
	{
		GameSaveLoader.LoadGameFile(loadFileNumber, this);
	}

	private void InitializeScene()
	{
		string sceneName = SceneManager.GetActiveScene().name;

		switch (sceneName)
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
		SceneManager.LoadSceneAsync(SceneNameTable.BATTLESTAGE, LoadSceneMode.Additive);
		SceneManager.LoadSceneAsync(SceneNameTable.CHAMP_STATUSBAR_UI, LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync(SceneNameTable.BANPICK_UI);
		battleStageManager.StartBattle();
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
		// ==========================================================================================
		// DataTable Manager ����..
		newGameObject = new GameObject("DataTable Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		dataTableManager = newGameObject.AddComponent<DataTableManager>();

		// Effect Manager ����..
		newGameObject = new GameObject("Effect Manager");
		DontDestroyOnLoad(newGameObject);
		newGameObject.transform.parent = transform;
		effectManager = newGameObject.AddComponent<EffectManager>();

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
		teamManager.gameManager = this;
		pilotManager.gameManager = this;
		effectManager.gameManager = this;
	}
}
