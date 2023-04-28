using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 모든 게임에 관련된 것들을 관리하기 위한 클래스..
/// </summary>
public class GameManager : MonoBehaviour
{
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

		LoadAdditiveScene();
	}

    private void LoadAdditiveScene()
	{
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

                PlayEnviormentSound(DORMITORY_ENVIORMENT_SOUND_NAME);

                break;

			case SceneNameTable.STADIUM:
				CreateBattleStageManager();
				CreateBanpickRunner();
				SceneManager.LoadScene(SceneNameTable.BANPICK_UI, LoadSceneMode.Additive);
				SceneManager.LoadScene(SceneNameTable.BATTLETEAM_INFO_UI, LoadSceneMode.Additive);
				//SceneManager.LoadScene(SceneNameTable.CHAMP_STATUSBAR_UI, LoadSceneMode.Additive);

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
		// 이전 씬에서 정리해야할 것들 정리..
		switch (_curSceneName)
		{
			case SceneNameTable.TITLE:
				battleStageManager.OnBattleEnd();
				battleStageManager.ExitBattleStage();
				Destroy(battleStageManager.gameObject);

				break;

            case SceneNameTable.STADIUM:
                Destroy(battleStageManager.gameObject);

                break;
        }

		_curSceneName = sceneName;

		// 현재 변경되야 할 씬에 맞게 초기화..
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

		// 추가 씬 로드(UI 같은 것들)..
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
        battleStageManager.OnEndBattle -= OnEndBattle;

		if (_curSceneName == SceneNameTable.STADIUM)
		{
			dataTableManager.statisticsDataTable.AddBattleTeamFightData(dataTableManager.battleStageDataTable.redTeamBattleFightData,
				dataTableManager.battleStageDataTable.blueTeamBattleFightData, winTeam);
		}
		else if (_curSceneName == SceneNameTable.TITLE)
		{
			Invoke("OnStartBattle", 0.5f);
		}
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

			Debug.Log($"{i + 1}번째 챔피언 골라부럿다.");
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
