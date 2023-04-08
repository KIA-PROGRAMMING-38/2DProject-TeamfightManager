using UnityEngine;

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

	// ����� ������ �ҷ����� �޼ҵ�..
	private void LoadFile(int loadFileNumber)
	{
		GameSaveLoader.LoadGameFile(loadFileNumber, dataTableManager);
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
		newGameObject.transform.parent = transform;
		dataTableManager = newGameObject.AddComponent<DataTableManager>();

		// Effect Manager ����..
		newGameObject = new GameObject("Effect Manager");
		newGameObject.transform.parent = transform;
		effectManager = newGameObject.AddComponent<EffectManager>();

		// Champion Manager ����..
		newGameObject = new GameObject("Champion Manager");
		newGameObject.transform.parent = transform;
		championManager = newGameObject.AddComponent<ChampionManager>();

		// Pilot Manager ����..
		newGameObject = new GameObject("pilot Manager");
		newGameObject.transform.parent = transform;
		pilotManager = newGameObject.AddComponent<PilotManager>();

		// Team Manager ����..
		newGameObject = new GameObject("Team Manager");
		newGameObject.transform.parent = transform;
		teamManager = newGameObject.AddComponent<TeamManager>();

		// UI Manager ����..
		newGameObject = new GameObject("UI Manager");
		newGameObject.transform.parent = transform;
		uiManager = newGameObject.AddComponent<UIManager>();

		// ���������� ���� �Ѱ��ֱ�..
		championManager.gameManager = this;
		teamManager.gameManager = this;
		pilotManager.gameManager = this;
		effectManager.gameManager = this;
		uiManager.gameManager = this;
	}
}
