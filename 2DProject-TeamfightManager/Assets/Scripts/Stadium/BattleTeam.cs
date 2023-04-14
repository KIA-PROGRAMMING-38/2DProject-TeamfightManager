using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ������������ ��Ʋ�� �ϴ� ��(�ϳ��� ��)�� �����ϱ� ���� Ŭ����..
/// </summary>
public class BattleTeam : MonoBehaviour
{
    public ChampionManager championManager { private get; set; }
    public PilotManager pilotManager { private get; set; }
	public BattleStageManager battleStageManager { private get; set; }
	public BattleTeam enemyTeam { private get; set; }
	public BattleStageDataTable battleStageDataTable { private get; set; }

	public Vector2[] spawnArea;
	private Vector3 randomSpawnPoint
	{
		get
		{
			return new Vector3(UnityEngine.Random.Range(spawnArea[0].x, spawnArea[1].x), UnityEngine.Random.Range(spawnArea[0].y, spawnArea[1].y), 0f);
		}
	}

	public BattleTeamKind battleTeamKind { private get; set; }

	private List<PilotBattle> _pilots = new List<PilotBattle>();
	private List<Champion> _activeChampions = new List<Champion>();
	private List<Champion> _allChampions = new List<Champion>();

	// è�Ǿ� ������ ���ŵ� ������ �ܺ��� �����ڵ鿡�� �˷��� �̺�Ʈ��..
	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleInfoData;
	public event Action<BattleTeamKind, int, float> OnChangedChampionHPRatio;
	public event Action<BattleTeamKind, int, float> OnChangedChampionMPRatio;
	public event Action<BattleTeamKind, int> OnChampionUseUltimate;
	public event Action<BattleTeamKind, int, float> OnChangedChampionBarrierRatio;

	// ��Ȱ ���� �ʵ��..
	private float _revivalWaitTime = 1f;
	private WaitForSeconds _revivalWaitSecInst;

	private List<IEnumerator> _revivalCoroutines = new List<IEnumerator>();

	private void Start()
	{
		_revivalWaitSecInst = YieldInstructionStore.GetWaitForSec(_revivalWaitTime);
	}

	/// <summary>
	/// �Ҽӵ� ���Ϸ��� �߰����ִ� �Լ�..
	/// ��Ʋ ������������ ���� ���Ϸ��� è�Ǿ��� �޾ƿ� �����Ѵ�..
	/// </summary>
	/// <param name="pilotName"></param>
	/// <param name="champName"></param>
	public void AddPilot(string pilotName, string champName)
    {
		// ������ �Ŵ������Լ� �ν��Ͻ��� �޾ƿ´�..
		Pilot pilot = pilotManager.GetPilotInstance(pilotName);
		Champion champion = championManager.GetChampionInstance(champName);

#if UNITY_EDITOR
		Debug.Assert(null != pilot);
		Debug.Assert(null != champion);
#endif

		PilotBattle pilotBattleComponent = pilot.battleComponent;

#if UNITY_EDITOR
		Debug.Assert(null != pilotBattleComponent);

		champion.gameObject.name = champName;
#endif

		// �ʱ�ȭ..
		pilotBattleComponent.controlChampion = champion;
		pilotBattleComponent.myTeam = this;
		pilotBattleComponent.battleTeamIndexKey = _pilots.Count;
		champion.transform.position = randomSpawnPoint;

		// Pilot Battle Component �� ���� ������ �ֱ�..
		_pilots.Add(pilotBattleComponent);
		_allChampions.Add(champion);
		_activeChampions.Add(champion);

		// ���Ϸ� �̺�Ʈ ����..
		pilotBattleComponent.OnChangedBattleInfoData -= OnChangedChampionBattleData;
		pilotBattleComponent.OnChangedBattleInfoData += OnChangedChampionBattleData;

		pilotBattleComponent.OnChangedChampionHPRatio -= UpdateChampionHPRatio;
		pilotBattleComponent.OnChangedChampionHPRatio += UpdateChampionHPRatio;

		pilotBattleComponent.OnChangedChampionMPRatio -= UpdateChampionMPRatio;
		pilotBattleComponent.OnChangedChampionMPRatio += UpdateChampionMPRatio;

		pilotBattleComponent.OnChampionUseUltimate -= UpdateChampionUseUltimateState;
		pilotBattleComponent.OnChampionUseUltimate += UpdateChampionUseUltimateState;

		pilotBattleComponent.OnChangedChampionBarrierRatio -= UpdateChampionBarrierRatio;
		pilotBattleComponent.OnChangedChampionBarrierRatio += UpdateChampionBarrierRatio;

		// �ڷ�ƾ ���..
		_revivalCoroutines.Add(WaitRevival(_pilots.Count - 1));
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
		_allChampions[pilotIndex].gameObject.SetActive(false);

		// Ȱ��ȭ ��Ͽ��� �����..
		int activeChampCount = _activeChampions.Count;
		for( int i = 0; i < activeChampCount; ++i)
		{
			if (_activeChampions[i] == _allChampions[pilotIndex])
			{
				_activeChampions.RemoveAt(i);
				break;
			}
		}

		battleStageManager.OnChampionDeadState(battleTeamKind, pilotIndex);

		StartCoroutine(_revivalCoroutines[pilotIndex]);
	}

	IEnumerator WaitRevival(int pilotIndex)
	{
		while(true)
		{
			yield return _revivalWaitSecInst;

			battleStageManager.OnChampionRevivalState(battleTeamKind, pilotIndex);

			OnSuccessRevival(_allChampions[pilotIndex]);

			StopCoroutine(_revivalCoroutines[pilotIndex]);

			yield return null;
		}
	}

	public void OnSuccessRevival(Champion champion)
	{
		champion.Revival();

		champion.transform.position = randomSpawnPoint;
		champion.gameObject.SetActive(true);

		_activeChampions.Add(champion);
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
