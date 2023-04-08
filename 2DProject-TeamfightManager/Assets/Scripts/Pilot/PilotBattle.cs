using UnityEngine;

/// <summary>
/// ���Ϸ��� ���� ���� ��ɵ��� ����ϴ� Ŭ����..
/// </summary>
public class PilotBattle : MonoBehaviour
{
    public Pilot pilot { get; private set; }
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

	private void Awake()
	{
		pilot = GetComponent<Pilot>();
	}

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
