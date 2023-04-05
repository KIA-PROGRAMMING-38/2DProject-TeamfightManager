using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// ��Ʋ ������������ ��Ʋ�� �ϴ� ��(�ϳ��� ��)�� �����ϱ� ���� Ŭ����..
/// </summary>
public class BattleTeam : MonoBehaviour
{
    public ChampionManager championManager { private get; set; }
    public PilotManager pilotManager { private get; set; }
	public BattleStageManager battleStageManager { private get; set; }
	public BattleTeam enemyTeam { private get; set; }

	public Vector2[] spawnArea;
	private Vector3 randomSpawnPoint
	{
		get
		{
			return new Vector3(Random.Range(spawnArea[0].x, spawnArea[1].x), Random.Range(spawnArea[0].y, spawnArea[1].y), 0f);
		}
	}

    private List<PilotBattle> _pilots = new List<PilotBattle>();
	private List<Champion> _activeChampions = new List<Champion>();
	private List<Champion> _allChampions = new List<Champion>();

    public void AddPilot(string pilotName, string champName)
    {
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

		// �ʱ�ȭ..
		pilotBattleComponent.controlChampion = champion;
		pilotBattleComponent.myTeam = this;
		champion.transform.position = randomSpawnPoint;

		// Pilot Battle Component �� ���� ������ �ֱ�..
		_pilots.Add(pilotBattleComponent);
		_allChampions.Add(champion);
		_activeChampions.Add(champion);
	}

	public void OnChampionDead(Champion champion)
	{
		champion.gameObject.SetActive(false);

		int activeChampCount = _activeChampions.Count;
		for( int i = 0; i < activeChampCount; ++i)
		{
			if (_activeChampions[i] == champion)
			{
				_activeChampions.RemoveAt(i);
				break;
			}
		}

		battleStageManager.OnChampionDead(this, champion);
	}

	public void OnSuccessRevival(Champion champion)
	{
		champion.Revival();

		champion.transform.position = randomSpawnPoint;
		champion.gameObject.SetActive(true);

		_activeChampions.Add(champion);
	}

	public void TestColorChange(Color color)
	{
		foreach (var pilot in enemyTeam._pilots)
		{
			pilot.controlChampion.TestColorChange(color);
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

	// originPoint�� �������� ���� ���� ���� ã�� �Լ�..
	public int ComputeInCircleRangeEnemyTarget(Vector3 originPoint, float radius, Champion[] championCache)
	{
		int enemyCount = 0;
		int championCacheLength = championCache.Length;

		int loopCount = enemyTeam._activeChampions.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			Champion enemy = enemyTeam._activeChampions[i];

			float dist = (originPoint - enemy.transform.position).magnitude;
			if (dist <= radius)
			{
				if (championCacheLength <= enemyCount)
					break;

				championCache[enemyCount++] = enemy;
			}
		}

		return enemyCount;
	}
}
