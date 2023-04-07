using UnityEngine;

/// <summary>
/// 파일럿의 전투 관련 기능들을 담당하는 클래스..
/// </summary>
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
    public BattleTeam myTeam { get; set; }

    public Champion FindTarget(Champion champion)
    {
        return myTeam.ComputeMostNearestEnemyTarget(champion.transform.position);
	}

    public void OnChampionDead(Champion champion)
    {
        myTeam.OnChampionDead(champion);
    }

    public void StopChampionLogic()
    {
        _controlChampion.GetComponent<ChampionController>().enabled = false;
        _controlChampion.GetComponentInChildren<ChampionAnimation>().ChangeState(ChampionAnimation.AnimState.Idle);
		_controlChampion.enabled = false;
	}
}
