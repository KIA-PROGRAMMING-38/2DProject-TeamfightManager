using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배틀 스테이지에서 배틀을 하는 팀(하나의 팀)을 관리하기 위한 클래스..
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

			SetupContainer();
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

	public BattleTeamKind battleTeamKind { private get; set; }

	private List<PilotBattle> _pilots;
	private List<Champion> _activeChampions;
	private List<Champion> _allChampions;

	// 챔피언 정보가 갱신될 때마다 외부의 구독자들에게 알려줄 이벤트들..
	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleInfoData;
	public event Action<BattleTeamKind, int, float> OnChangedChampionHPRatio;
	public event Action<BattleTeamKind, int, float> OnChangedChampionMPRatio;
	public event Action<BattleTeamKind, int> OnChampionUseUltimate;
	public event Action<BattleTeamKind, int, float> OnChangedChampionBarrierRatio;

	// 부활 관련 필드들..
	private float _revivalWaitTime = 3f;
	private WaitForSeconds _revivalWaitSecInst;

	private List<IEnumerator> _revivalCoroutines = new List<IEnumerator>();

	private void Awake()
	{
		
	}

	private void Start()
	{
		_revivalWaitSecInst = YieldInstructionStore.GetWaitForSec(_revivalWaitTime);
	}

	private void SetupContainer()
	{
		int battleChampCount = _battleStageDataTable.battleChampionTotalCount / 2;

		_pilots = new List<PilotBattle>(battleChampCount);
		_activeChampions = new List<Champion>(battleChampCount);
		_allChampions = new List<Champion>(battleChampCount);

		for (int i = 0; i < battleChampCount; ++i)
		{
			_pilots.Add(null);
			_allChampions.Add(null);
		}
	}

	/// <summary>
	/// 소속될 파일럿을 추가해주는 함수..
	/// 배틀 스테이지에서 사용될 파일럿과 챔피언을 받아와 저장한다..
	/// </summary>
	/// <param name="pilotName"></param>
	/// <param name="champName"></param>
	public void AddPilot(int index, string pilotName)
    {
		// 매니저에게서 인스턴스를 받아온다..
		Pilot pilot = _pilotManager.GetPilotInstance(pilotName);

#if UNITY_EDITOR
		Debug.Assert(null != pilot);
#endif

		PilotBattle pilotBattleComponent = pilot.battleComponent;

#if UNITY_EDITOR
		Debug.Assert(null != pilotBattleComponent);
#endif

		// 초기화..
		pilotBattleComponent.myTeam = this;
		pilotBattleComponent.battleTeamIndexKey = index;

		// Pilot Battle Component 를 나의 팀으로 넣기..
		_pilots[index] = pilotBattleComponent;

		// 파일럿 이벤트 연결..
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

		// 코루틴 등록..
		_revivalCoroutines.Add(WaitRevival(_pilots.Count - 1));
	}

	public void AddChampion(int index, string championName)
	{
		// 매니저에게서 인스턴스를 받아온다..
		Champion champion = _championManager.GetChampionInstance(championName);
		PilotBattle pilotBattleComponent = _pilots[index];

#if UNITY_EDITOR
		Debug.Assert(null != champion);
		Debug.Assert(null != pilotBattleComponent);

		champion.gameObject.name = championName;
#endif

		// 초기화..
		pilotBattleComponent.controlChampion = champion;
		champion.transform.position = randomSpawnPoint;
		champion.gameObject.SetActive(false);

		// 챔피언 목록 및 활성화 챔피언 목록에 추가..
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
		// 활성화 목록에서 지운다..
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

			_battleStageManager.OnChampionRevivalState(battleTeamKind, pilotIndex);

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

		// 부활 이펙트 On..
		Effect effect = _effectManager.GetEffect("Effect_Revival", champion.transform.position);
		effect.SetupAdditionalData(Vector3.zero, champion.transform);
		effect.gameObject.SetActive(true);

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

	private void UpdateChampionBarrierRatio(int index, float ratio)
	{
		OnChangedChampionBarrierRatio?.Invoke(battleTeamKind, index, ratio);
	}
}
