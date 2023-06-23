using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ��� ���ӿ� ���õ� �͵��� �����ϱ� ���� Ŭ����..
/// </summary>
public class GameManager : MonoBehaviour
{
	public static string s_currentSceneName { get; private set; }

    private const string TITLE_ENVIORMENT_SOUND_NAME = "Title_Enviorment";
    private const string BANPICK_ENVIORMENT_SOUND_NAME = "Banpick_Enviorment";
    private const string BATTLESTAGE_ENVIORMENT_SOUND_NAME = "BattleStage_Enviorment";
    private const string DORMITORY_ENVIORMENT_SOUND_NAME = "Dormitory_Enviorment";

    public event Action OnChangeScene;

	private static GameManager instance;

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

	private AudioSource _audioSource;

	private void Awake()
	{
		if (null != instance)
		{
			Destroy(gameObject);

			return;
		}
		else
		{
			instance = this;
		}

		Screen.SetResolution(1920, 1080, true);

        _audioSource = GetComponent<AudioSource>();

        UIBase.s_gameManager = this;

		DontDestroyOnLoad(gameObject);

        LoadFile( 0 );
        SetupManager();

		InitializeFloatingDamageUISpanwer();
	}

	private void Start()
	{
		InitializeScene();
	}

	public static bool isAutoPlaying = false;
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.F3))
		{
			isAutoPlaying = !isAutoPlaying;
		}
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

		dataTableManager.OnEndReadData();
	}

	private void InitializeScene()
	{
        _curSceneName = SceneManager.GetActiveScene().name;

		LoadAdditiveScene();
	}

    private void LoadAdditiveScene()
	{
		s_currentSceneName = _curSceneName;

		switch (_curSceneName)
		{
			case SceneNameTable.TITLE:
				SceneManager.LoadScene(SceneNameTable.TITLE_FIGHT, LoadSceneMode.Additive);
				CreateBattleStageManager();
				OnStartBattle();

				PlayEnviormentSound(TITLE_ENVIORMENT_SOUND_NAME);

				break;

			case SceneNameTable.DORMITORY:
				SceneManager.LoadScene(SceneNameTable.DORMITORY_UI, LoadSceneMode.Additive);
				SceneManager.LoadScene(SceneNameTable.PAUSE_UI, LoadSceneMode.Additive);

                PlayEnviormentSound(DORMITORY_ENVIORMENT_SOUND_NAME);

                break;

			case SceneNameTable.STADIUM:
				CreateBattleStageManager();
				CreateBanpickRunner();
				SceneManager.LoadScene(SceneNameTable.BANPICK_UI, LoadSceneMode.Additive);
				SceneManager.LoadScene(SceneNameTable.BATTLETEAM_INFO_UI, LoadSceneMode.Additive);
				SceneManager.LoadScene(SceneNameTable.PAUSE_UI, LoadSceneMode.Additive);

				dataTableManager.battleStageDataTable.OnStartBattle -= OnStartBattle;
				dataTableManager.battleStageDataTable.OnStartBattle += OnStartBattle;

                PlayEnviormentSound(BANPICK_ENVIORMENT_SOUND_NAME);
                break;
		}
	}

	private void PlayEnviormentSound(string soundName)
	{
		AudioClip enviormentClip = SoundStore.GetAudioClip(soundName);
		_audioSource.clip = enviormentClip;
		_audioSource.Play();
	}

	public void ChangeScene(string sceneName)
	{
		// ���� ������ �����ؾ��� �͵� ����..
		switch (_curSceneName)
		{
			case SceneNameTable.TITLE:
				battleStageManager.OnBattleEnd();
				battleStageManager.ExitBattleStage();
				Destroy(battleStageManager.gameObject);

				break;

            case SceneNameTable.STADIUM:
                Destroy(battleStageManager.gameObject);
				Destroy(banpickRunner.gameObject);

				break;
        }

		_curSceneName = sceneName;

		// ���� ����Ǿ� �� ���� �°� �ʱ�ȭ..
		switch (_curSceneName)
		{
			case SceneNameTable.TITLE:
				SceneManager.LoadScene(_curSceneName, LoadSceneMode.Single);

				break;

			case SceneNameTable.DORMITORY:
				SceneManager.LoadScene(_curSceneName, LoadSceneMode.Single);

				break;

			case SceneNameTable.STADIUM:
				SceneManager.LoadScene(_curSceneName, LoadSceneMode.Single);

				break;
		}

		// �߰� �� �ε�(UI ���� �͵�)..
		LoadAdditiveScene();

		OnChangeScene?.Invoke();
	}

	private void OnStartBattle()
	{
		if (_curSceneName == SceneNameTable.STADIUM)
		{
			PlayEnviormentSound(BATTLESTAGE_ENVIORMENT_SOUND_NAME);

            SceneManager.LoadSceneAsync(SceneNameTable.CHAMP_STATUSBAR_UI, LoadSceneMode.Additive);
			SceneManager.UnloadSceneAsync(SceneNameTable.BANPICK_UI);
			SceneManager.LoadSceneAsync(SceneNameTable.BATTLESTAGE, LoadSceneMode.Additive);
		}
		else if (_curSceneName == SceneNameTable.TITLE)
		{
			SetupTitleFight();
		}

		battleStageManager.StartBattle();

		battleStageManager.OnEndBattle -= OnEndBattle;
		battleStageManager.OnEndBattle += OnEndBattle;
    }

	private void OnEndBattle(BattleTeamKind winTeam)
	{
		BattleStageDataTable dataTable = dataTableManager.battleStageDataTable;

        battleStageManager.OnEndBattle -= OnEndBattle;

		// ���� ���� ���� �ؾ��� �ൿ�� �Ѵ�..
		if (_curSceneName == SceneNameTable.STADIUM)
		{
			BattleStatisticsDataTable statisticsDataTable = dataTableManager.statisticsDataTable;

			// ��踦 ����ϴ� �ν��Ͻ����� ������ ����..
			statisticsDataTable.AddBattleTeamFightData( dataTable.redTeamBattleFightData, dataTable.blueTeamBattleFightData, winTeam);
		}
		else if (_curSceneName == SceneNameTable.TITLE)
		{
			// �ٽ� è�Ǿ��� ������ ���� ���� ���� ����..
			StartCoroutine(StartTitleBattle());
		}
    }

	IEnumerator StartTitleBattle()
	{
		yield return YieldInstructionStore.GetWaitForSec(0.5f);

		dataTableManager.battleStageDataTable.Initialize(battleStageManager.redTeam.teamName, battleStageManager.redTeam.battlePilotFightData,
			battleStageManager.blueTeam.teamName, battleStageManager.blueTeam.battlePilotFightData);

		OnStartBattle();
	}

	private void SetupTitleFight()
	{
		HashSet<string> pickChampionList = new HashSet<string>();

		ChampionDataTable championDataTable = dataTableManager.championDataTable;
		int championMaxCount = championDataTable.GetTotalChampionCount();

		for (int i = 0; i < gameGlobalData.PilotCount; ++i)
		{
			string champName = "";

			while (true)
			{
				int randomIndex = UnityEngine.Random.Range(0, championMaxCount);
				champName = championDataTable.GetChampionName(randomIndex);
				if (false == pickChampionList.Contains(champName) && false == champName.Contains("Jako"))
					break;
			}

			pickChampionList.Add(champName);
			battleStageManager.PickChampion(BattleTeamKind.BlueTeam, i, champName);

			while (true)
			{
				int randomIndex = UnityEngine.Random.Range(0, championMaxCount);
				champName = championDataTable.GetChampionName(randomIndex);
				if (false == pickChampionList.Contains(champName) && false == champName.Contains("Jako"))
					break;
			}

			pickChampionList.Add(champName);
			battleStageManager.PickChampion(BattleTeamKind.RedTeam, i, champName);
		}
	}

	private void InitializeFloatingDamageUISpanwer()
	{
		FloatingDamageUI damageUIComponent = _gameGlobalData.floatingDamageUIPrefab.GetComponent<FloatingDamageUI>();
#if UNITY_EDITOR
		Debug.Assert(null != damageUIComponent);
#endif

		FloatingDamageUISpawner.gameManager = this;
        FloatingDamageUISpawner.Initialize(() =>
		{
			FloatingDamageUI ui = Instantiate(damageUIComponent);
			DontDestroyOnLoad(ui.gameObject);

			return ui;
        });
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
		newGameObject.transform.parent = transform;
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
