using UnityEngine;

public class PilotBattle : MonoBehaviour
{
    public Champion controlChampion
    {
        get => _controlChampion;
		set
        {
            _controlChampion = value;

            _controlChampion.pilotBattleComponent = this;
		}
    }
    private Champion _controlChampion;
    public BattleTeam myTeam { private get; set; }

    public GameObject FindTarget(Champion champion)
    {
        return myTeam.ComputeMostNearestEnemyTarget(champion.transform.position);
	}

    public void OnChampionDead(Champion champion)
    {
        myTeam.OnChampionDead(champion);
    }
}
