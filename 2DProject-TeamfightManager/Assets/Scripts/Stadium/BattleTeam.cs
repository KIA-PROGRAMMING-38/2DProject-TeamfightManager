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

	public Vector2[] spawnArea;
	private Vector3 randomSpawnPoint
	{
		get
		{
			return new Vector3(Random.Range(spawnArea[0].x, spawnArea[1].x), Random.Range(spawnArea[0].y, spawnArea[1].y), 0f);
		}
	}

    private List<PilotBattle> _pilots = new List<PilotBattle>();

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

		// 초기화..
		pilotBattleComponent.controlChampion = champion;
		pilotBattleComponent.myTeam = this;
		champion.transform.position = randomSpawnPoint;

		// Pilot Battle Component 를 나의 팀으로 넣기..
		_pilots.Add(pilotBattleComponent);
	}

	public GameObject ComputeMostNearestEnemyTarget( Vector3 originPoint )
    {
		float result = float.MaxValue;
		GameObject target = null;
		foreach(var pilot in enemyTeam._pilots)
		{
			if (pilot.controlChampion.isDead)
				continue;

			float dist = (originPoint - pilot.controlChampion.transform.position).sqrMagnitude;
			if (dist < result)
			{
				result = dist;
				target = pilot.controlChampion.gameObject;
			}
		}

        return target;
	}

	public void OnChampionDead(Champion champion)
	{
		champion.gameObject.SetActive(false);

		battleStageManager.OnChampionDead(this, champion);
	}

	public void OnSuccessRevival(Champion champion)
	{
		champion.Revival();

		champion.transform.position = randomSpawnPoint;
		champion.gameObject.SetActive(true);
	}

	public void TestColorChange(Color color)
	{
		foreach (var pilot in enemyTeam._pilots)
		{
			pilot.controlChampion.TestColorChange(color);
		}
	}
}
