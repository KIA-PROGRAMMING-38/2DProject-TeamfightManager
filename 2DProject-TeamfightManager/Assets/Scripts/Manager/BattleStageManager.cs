using System;
using UnityEngine;

public enum BattleTeamKind
{
	RedTeam,
	BlueTeam,
	End
}

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
			_battleStageDataTable.battleStageManager = this;
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
		redTeam.battleTeamKind = BattleTeamKind.RedTeam;
		redTeam.enemyTeam = blueTeam;
		redTeam.championManager = _championManager;
		redTeam.pilotManager = _pilotManager;
		redTeam.battleStageManager = this;
		redTeam.spawnArea = _redTeamSpawnArea;

		blueTeam.battleTeamKind = BattleTeamKind.BlueTeam;
		blueTeam.enemyTeam = redTeam;
		blueTeam.championManager = _championManager;
		blueTeam.pilotManager = _pilotManager;
		blueTeam.battleStageManager = this;
		blueTeam.spawnArea = _blueTeamSpawnArea;

		// �� �� ������Ʈ�� �̺�Ʈ ����..
		redTeam.OnChangedChampionBattleInfoData -= OnChangedChampionBattleData;
		redTeam.OnChangedChampionBattleInfoData += OnChangedChampionBattleData;

		redTeam.OnChangedChampionHPRatio -= OnUpdateChampionHPRatio;
		redTeam.OnChangedChampionHPRatio += OnUpdateChampionHPRatio;

		redTeam.OnChangedChampionMPRatio -= OnUpdateChampionMPRatio;
		redTeam.OnChangedChampionMPRatio += OnUpdateChampionMPRatio;

		redTeam.OnChampionUseUltimate -= OnChampionUseUltimate;
		redTeam.OnChampionUseUltimate += OnChampionUseUltimate;


		blueTeam.OnChangedChampionBattleInfoData -= OnChangedChampionBattleData;
		blueTeam.OnChangedChampionBattleInfoData += OnChangedChampionBattleData;

		blueTeam.OnChangedChampionHPRatio -= OnUpdateChampionHPRatio;
		blueTeam.OnChangedChampionHPRatio += OnUpdateChampionHPRatio;

		blueTeam.OnChangedChampionMPRatio -= OnUpdateChampionMPRatio;
		blueTeam.OnChangedChampionMPRatio += OnUpdateChampionMPRatio;

		blueTeam.OnChampionUseUltimate -= OnChampionUseUltimate;
		blueTeam.OnChampionUseUltimate += OnChampionUseUltimate;
	}

	// ��Ʋ ���������� �� ������ ���Ϸ� �������ִ� �Լ�..
	private void SetupPilot()
	{
		int pilotCount = 1;
		_battleStageDataTable.battleChampionTotalCount = pilotCount * 2;

		for (int i = 0; i < pilotCount; ++i)
		{
			redTeam.AddPilot("Faker", "Swordman");
			blueTeam.AddPilot("Faker", "Swordman");
		}

		redTeam.TestColorChange(Color.red);
		blueTeam.TestColorChange(Color.blue);
	}

	// �� ������ �ε����� �޾ƿ� è�Ǿ��� �̸��� �����ϴ� �Լ�..
	public string GetChampionName(BattleTeamKind teamKind, int index)
	{
		return GetChampion(teamKind, index).data.name;
	}

	public Champion GetChampion(BattleTeamKind teamKind, int index)
	{
		if (teamKind == BattleTeamKind.RedTeam)
			return redTeam.GetChampion(index);
		else
			return blueTeam.GetChampion(index);
	}

	public Pilot GetPilot(BattleTeamKind teamKind, int index)
	{
		if (teamKind == BattleTeamKind.RedTeam)
			return redTeam.GetPilot(index);
		else
			return blueTeam.GetPilot(index);
	}

	// ��Ʋ ���� �ð� ���ŵǸ� ȣ��Ǵ� �ݹ� �Լ�..
	private void OnUpdateBattleRemainTime(float remainTime)
	{
		// �� �κ��� �׽�Ʈ�� �ڵ��(UI �۾� �� UI ��ũ��Ʈ���� ó���ϵ��� �ٲ�� �Ѵ�)
		UnityEngine.UI.Text _remainTimeText = GameObject.Find("remainingTimeText").GetComponent<UnityEngine.UI.Text>();

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
#if UNITY_EDITOR
		Debug.Log("��Ʋ�� ����Ǿ���.");
#endif

		StopAllCoroutines();

		redTeam.OnBattleEnd();
		blueTeam.OnBattleEnd();
	}

	// ��Ʋ ���������� è�Ǿ���� ��Ʋ ������ �ٲ� ������ ȣ��ȴ�..
	private void OnChangedChampionBattleData(BattleTeamKind teamKind, int index, BattleInfoData data)
	{
		_battleStageDataTable?.ModifyChampionBattleData(teamKind, index, data);
	}

	private void OnUpdateChampionHPRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		_battleStageDataTable?.ModifyChampionHPRatio(teamKind, index, ratio);
	}

	private void OnUpdateChampionMPRatio(BattleTeamKind teamKind, int index, float ratio)
	{
		_battleStageDataTable?.ModifyChampionMPRatio(teamKind, index, ratio);
	}

	private void OnChampionUseUltimate(BattleTeamKind teamKind, int index)
	{
		_battleStageDataTable?.ModifyChampionUseUltimateState(teamKind, index);
	}

	public void OnChampionDeadState(BattleTeamKind teamKind, int index)
	{
		_battleStageDataTable?.OnChampionDeath(teamKind, index);
	}

	public void OnChampionRevivalState(BattleTeamKind teamKind, int index)
	{
		_battleStageDataTable?.OnChampionRevival(teamKind, index);
	}
}
