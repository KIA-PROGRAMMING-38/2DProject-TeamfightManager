using System.Collections.Generic;
using UnityEngine;

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
}
