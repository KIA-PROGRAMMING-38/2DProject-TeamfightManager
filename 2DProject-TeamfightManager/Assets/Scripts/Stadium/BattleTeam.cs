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

	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleInfoData;

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
	}

	public void OnChampionDead(Champion champion)
	{
		champion.gameObject.SetActive(false);

		// 활성화 목록에서 지운다..
		int activeChampCount = _activeChampions.Count;
		for( int i = 0; i < activeChampCount; ++i)
		{
			if (_activeChampions[i] == champion)
			{
				_activeChampions.RemoveAt(i);
				break;
			}
		}

		StartCoroutine( WaitRevival( champion ) );
	}

	IEnumerator WaitRevival(Champion champion)
	{
		yield return YieldInstructionStore.GetWaitForSec( 1f );

		OnSuccessRevival( champion );
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

	public void TestColorChange(Color color)
	{
		foreach (var pilot in enemyTeam._pilots)
		{
			pilot.controlChampion.TestColorChange(color);
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
	public int ComputeEnemyTarget(Func<Vector3, bool> findLogicFunction, Champion[] championCache)
	{
		int enemyCount = 0;
		int championCacheLength = championCache.Length;

		int loopCount = enemyTeam._activeChampions.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			Champion enemy = enemyTeam._activeChampions[i];

			if( true == findLogicFunction(enemy.transform.position) )
			{
				if (championCacheLength <= enemyCount)
					break;

				championCache[enemyCount++] = enemy;
			}
		}

		return enemyCount;
	}

	private void OnChangedChampionBattleData(int index, BattleInfoData data)
	{
		OnChangedChampionBattleInfoData?.Invoke(battleTeamKind, index, data);
	}
}
