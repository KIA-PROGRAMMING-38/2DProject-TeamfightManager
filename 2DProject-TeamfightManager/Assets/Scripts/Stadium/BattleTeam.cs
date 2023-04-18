using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ������������ ��Ʋ�� �ϴ� ��(�ϳ��� ��)�� �����ϱ� ���� Ŭ����..
/// </summary>
public class BattleTeam : MonoBehaviour
{
	public GameManager gameManager
	{
		set
		{
			_championManager = value.championManager;
			_pilotManager = value.pilotManager;
			_battleStageManager = value.battleStageManager;
			_battleStageDataTable = value.dataTableManager.battleStageDataTable;
			_effectManager = value.effectManager;
		}
	}

	public List<Pilot> belongPilot
	{
		set
		{
			int pilotCount = value.Count;

			_pilots = new List<PilotBattle>(pilotCount);
			_activeChampions = new List<Champion>(pilotCount);
			_allChampions = new List<Champion>(pilotCount);

			for( int i = 0; i < pilotCount; ++i)
			{
				PilotBattle pilotBattle = value[i].GetComponent<PilotBattle>();

				_pilots.Add(pilotBattle);
				_allChampions.Add(null);

				SetupPilot(pilotBattle, i);
			}
		}
	}

	private ChampionManager _championManager;
	private PilotManager _pilotManager;
	private BattleStageManager _battleStageManager;
	private BattleStageDataTable _battleStageDataTable;
	private EffectManager _effectManager;

	public BattleTeam enemyTeam { private get; set; }

	public Vector2[] spawnArea;
	private Vector3 randomSpawnPoint
	{
		get
		{
			return new Vector3(UnityEngine.Random.Range(spawnArea[0].x, spawnArea[1].x), UnityEngine.Random.Range(spawnArea[0].y, spawnArea[1].y), 0f);
		}
	}

	private Team _teamComponent;
	public string teamName { get => _teamComponent.data.name; }

	public BattleTeamKind battleTeamKind { private get; set; }

	private List<PilotBattle> _pilots;
	public List<BattlePilotFightData> battlePilotFightData
	{
		get
		{
			List<BattlePilotFightData> result = new List<BattlePilotFightData>();
			for (int i = 0; i < _pilots.Count; ++i)
				result.Add(_pilots[i].battlePilotFightData);

			return result;
		}
	}
	private List<Champion> _activeChampions;
	private List<Champion> _allChampions;

	// è�Ǿ� ������ ���ŵ� ������ �ܺ��� �����ڵ鿡�� �˷��� �̺�Ʈ��..
	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleInfoData;
	public event Action<BattleTeamKind, int, float> OnChangedChampionHPRatio;
	public event Action<BattleTeamKind, int, float> OnChangedChampionMPRatio;
	public event Action<BattleTeamKind, int> OnChampionUseUltimate;
	public event Action<BattleTeamKind, int, float> OnChangedChampionBarrierRatio;

	// ��Ȱ ���� �ʵ��..
	private float _revivalWaitTime = 3.5f;
	private float _revivalDelayTime = 0.5f;
    private WaitForSeconds _revivalWaitSecInst;
	private WaitForSeconds _revivalDelaySecInst;

    private List<IEnumerator> _revivalCoroutines = new List<IEnumerator>();

	private void Awake()
	{
		_teamComponent = GetComponent<Team>();
	}

	private void Start()
	{
		_revivalWaitSecInst = YieldInstructionStore.GetWaitForSec(_revivalWaitTime);
		_revivalDelaySecInst = YieldInstructionStore.GetWaitForSec( _revivalDelayTime );
    }

	/// <summary>
	/// �Ҽӵ� ���Ϸ��� �߰����ִ� �Լ�..
	/// ��Ʋ ������������ ���� ���Ϸ��� è�Ǿ��� �޾ƿ� �����Ѵ�..
	/// </summary>
	/// <param name="pilotName"></param>
	/// <param name="champName"></param>
	private void SetupPilot(PilotBattle pilotBattle, int index)
    {
#if UNITY_EDITOR
		Debug.Assert(null != pilotBattle);
#endif
		// �ʱ�ȭ..
		pilotBattle.myTeam = this;
		pilotBattle.battleTeamIndexKey = index;

		// Pilot Battle Component �� ���� ������ �ֱ�..
		_pilots[index] = pilotBattle;

		// ���Ϸ� �̺�Ʈ ����..
		pilotBattle.OnChangedBattleInfoData -= OnChangedChampionBattleData;
		pilotBattle.OnChangedBattleInfoData += OnChangedChampionBattleData;

		pilotBattle.OnChangedChampionHPRatio -= UpdateChampionHPRatio;
		pilotBattle.OnChangedChampionHPRatio += UpdateChampionHPRatio;

		pilotBattle.OnChangedChampionMPRatio -= UpdateChampionMPRatio;
		pilotBattle.OnChangedChampionMPRatio += UpdateChampionMPRatio;

		pilotBattle.OnChampionUseUltimate -= UpdateChampionUseUltimateState;
		pilotBattle.OnChampionUseUltimate += UpdateChampionUseUltimateState;

		pilotBattle.OnChangedChampionBarrierRatio -= UpdateChampionBarrierRatio;
		pilotBattle.OnChangedChampionBarrierRatio += UpdateChampionBarrierRatio;

		// �ڷ�ƾ ���..
		_revivalCoroutines.Add(WaitRevival(_pilots.Count - 1));
	}

	public void AddChampion(int index, string championName)
	{
		// �Ŵ������Լ� �ν��Ͻ��� �޾ƿ´�..
		Champion champion = _championManager.GetChampionInstance(championName);
		PilotBattle pilotBattleComponent = _pilots[index];

#if UNITY_EDITOR
		Debug.Assert(null != champion);
		Debug.Assert(null != pilotBattleComponent);

		champion.gameObject.name = championName;
#endif

		// �ʱ�ȭ..
		pilotBattleComponent.controlChampion = champion;
		champion.transform.position = randomSpawnPoint;
		champion.gameObject.SetActive(false);

		// è�Ǿ� ��� �� Ȱ��ȭ è�Ǿ� ��Ͽ� �߰�..
		_allChampions[index] = champion;
	}

	public void StartBattle()
	{
		int loopCount = _allChampions.Count;
		for( int i = 0; i < loopCount; ++i)
		{
			_allChampions[i].gameObject.SetActive(true);
			_activeChampions.Add(_allChampions[i]);
		}
	}

	public Champion GetChampion(int index)
	{
		return _pilots[index].controlChampion;
	}

	public Pilot GetPilot(int index)
	{
		return _pilots[index].pilot;
	}

	public void OnChampionDead(int pilotIndex)
	{
		// Ȱ��ȭ ��Ͽ��� �����..
		int activeChampCount = _activeChampions.Count;
		for( int i = 0; i < activeChampCount; ++i)
		{
			if (_activeChampions[i] == _allChampions[pilotIndex])
			{
				_activeChampions.RemoveAt(i);

				_battleStageManager.OnChampionDeadState(battleTeamKind, pilotIndex);

				StartCoroutine(_revivalCoroutines[pilotIndex]);

				break;
			}
		}
	}

	IEnumerator WaitRevival(int pilotIndex)
	{
		while(true)
		{
			yield return _revivalWaitSecInst;

			// ��Ȱ ����Ʈ On..
			Vector3 spawnPosition = randomSpawnPoint;
            Effect effect = _effectManager.GetEffect("Effect_Revival", spawnPosition);
            effect.gameObject.SetActive( true );

            yield return _revivalDelaySecInst;

            _battleStageManager.OnChampionRevivalState(battleTeamKind, pilotIndex);

			OnSuccessRevival(_allChampions[pilotIndex], spawnPosition);

			StopCoroutine(_revivalCoroutines[pilotIndex]);

			yield return null;
		}
	}

	public void OnSuccessRevival(Champion champion, in Vector3 spawnPosition)
	{
		champion.Revival();

		champion.transform.position = spawnPosition;
		champion.gameObject.SetActive(true);

        _activeChampions.Add( champion );
    }

	public void OnBattleEnd()
	{
		StopAllCoroutines();

		int loopCount = _pilots.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			_pilots[i].StopChampionLogic();
		}
	}

	// ==================================== ���� ã�� �Լ���.. ====================================

	// originPoint�� �������� ���� ����� ���� ã�� �Լ�..
	public Champion ComputeMostNearestEnemyTarget(Vector3 originPoint)
	{
		float result = float.MaxValue;
		Champion target = null;
		int loopCount = enemyTeam._activeChampions.Count;
		for( int i = 0; i < loopCount; ++i)
		{
			Champion enemy = enemyTeam._activeChampions[i];

			float dist = (originPoint - enemy.transform.position).sqrMagnitude;
			if (dist < result)
			{
				result = dist;
				target = enemy;
			}
		}

		return target;
	}

	// ���� ã�� ������ �޾ƿ� ����Ѵ�..
	public int ComputeEnemyTarget(Func<Vector3, bool> findLogicFunction, Champion[] championCache, TargetTeamKind teamKind)
	{
		int targetCount = 0;
		int championCacheLength = championCache.Length;

		List<Champion> computeContainer = null;
		switch (teamKind)
		{
			case TargetTeamKind.Enemy:
				computeContainer = enemyTeam._activeChampions;
				break;
			case TargetTeamKind.Team:
				computeContainer = _activeChampions;
				break;

			default:
				return 0;
		}

		int loopCount = computeContainer.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			Champion target = computeContainer[i];

			if( true == findLogicFunction(target.transform.position) )
			{
				if (championCacheLength <= targetCount)
					break;

				championCache[targetCount++] = target;
			}
		}

		return targetCount;
	}

	private void OnChangedChampionBattleData(int index, BattleInfoData data)
	{
		OnChangedChampionBattleInfoData?.Invoke(battleTeamKind, index, data);
	}

	private void UpdateChampionHPRatio(int index, float ratio)
	{
		OnChangedChampionHPRatio?.Invoke(battleTeamKind, index, ratio);
	}

	private void UpdateChampionMPRatio(int index, float ratio)
	{
		OnChangedChampionMPRatio?.Invoke(battleTeamKind, index, ratio);
	}

	private void UpdateChampionUseUltimateState(int index)
	{
		OnChampionUseUltimate?.Invoke(battleTeamKind, index);
	}

	private void UpdateChampionBarrierRatio(int index, float ratio)
	{
		OnChangedChampionBarrierRatio?.Invoke(battleTeamKind, index, ratio);
	}
}
