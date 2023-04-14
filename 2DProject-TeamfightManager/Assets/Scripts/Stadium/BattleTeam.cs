using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배틀 스테이지에서 배틀을 하는 팀(하나의 팀)을 관리하기 위한 클래스..
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

	// 챔피언 정보가 갱신될 때마다 외부의 구독자들에게 알려줄 이벤트들..
	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleInfoData;
	public event Action<BattleTeamKind, int, float> OnChangedChampionHPRatio;
	public event Action<BattleTeamKind, int, float> OnChangedChampionMPRatio;
	public event Action<BattleTeamKind, int> OnChampionUseUltimate;

	// 부활 관련 필드들..
	private float _revivalWaitTime = 1f;
	private WaitForSeconds _revivalWaitSecInst;

	private List<IEnumerator> _revivalCoroutines = new List<IEnumerator>();

	private void Start()
	{
		_revivalWaitSecInst = YieldInstructionStore.GetWaitForSec(_revivalWaitTime);
	}

	/// <summary>
	/// 소속될 파일럿을 추가해주는 함수..
	/// 배틀 스테이지에서 사용될 파일럿과 챔피언을 받아와 저장한다..
	/// </summary>
	/// <param name="pilotName"></param>
	/// <param name="champName"></param>
	public void AddPilot(string pilotName, string champName)
    {
		// 각각의 매니저에게서 인스턴스를 받아온다..
		Pilot pilot = pilotManager.GetPilotInstance(pilotName);
		Champion champion = championManager.GetChampionInstance(champName);

#if UNITY_EDITOR
		Debug.Assert(null != pilot);
		Debug.Assert(null != champion);
#endif

		PilotBattle pilotBattleComponent = pilot.battleComponent;

#if UNITY_EDITOR
		Debug.Assert(null != pilotBattleComponent);
#endif

		// 초기화..
		pilotBattleComponent.controlChampion = champion;
		pilotBattleComponent.myTeam = this;
		pilotBattleComponent.battleTeamIndexKey = _pilots.Count;
		champion.transform.position = randomSpawnPoint;

		// Pilot Battle Component 를 나의 팀으로 넣기..
		_pilots.Add(pilotBattleComponent);
		_allChampions.Add(champion);
		_activeChampions.Add(champion);

		// 파일럿 이벤트 연결..
		pilotBattleComponent.OnChangedBattleInfoData -= OnChangedChampionBattleData;
		pilotBattleComponent.OnChangedBattleInfoData += OnChangedChampionBattleData;

		pilotBattleComponent.OnChangedChampionHPRatio -= UpdateChampionHPRatio;
		pilotBattleComponent.OnChangedChampionHPRatio += UpdateChampionHPRatio;

		pilotBattleComponent.OnChangedChampionMPRatio -= UpdateChampionMPRatio;
		pilotBattleComponent.OnChangedChampionMPRatio += UpdateChampionMPRatio;

		pilotBattleComponent.OnChampionUseUltimate -= UpdateChampionUseUltimateState;
		pilotBattleComponent.OnChampionUseUltimate += UpdateChampionUseUltimateState;

		// 코루틴 등록..
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

		// 활성화 목록에서 지운다..
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

	// ==================================== 적을 찾는 함수들.. ====================================

	// originPoint를 기준으로 가장 가까운 적을 찾는 함수..
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

	// 적을 찾는 로직을 받아와 계산한다..
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
}
