using System;
using UnityEngine;

/// <summary>
/// 파일럿의 전투 관련 기능들을 담당하는 클래스..
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

            _controlChampion.OnHit -= OnChampionTakeDamaged;
            _controlChampion.OnHit += OnChampionTakeDamaged;

			_controlChampion.OnAttack -= OnChampionAttack;
			_controlChampion.OnAttack += OnChampionAttack;
		}
    }
    private Champion _controlChampion;
    public BattleTeam myTeam { get; set; }

    public int battleTeamIndexKey { private get; set; }

    private BattleInfoData _battleInfoData;

    public event Action<int, BattleInfoData> OnChangedBattleInfoData;

	private void Awake()
	{
		pilot = GetComponent<Pilot>();
		_battleInfoData = new BattleInfoData();
	}

	public Champion FindTarget(Champion champion)
    {
        return myTeam.ComputeMostNearestEnemyTarget(champion.transform.position);
	}

    public void OnChampionDead(Champion champion)
    {
        myTeam.OnChampionDead(champion);

        ++_battleInfoData.deathCount;
    }

    public void StopChampionLogic()
    {
        _controlChampion.GetComponent<ChampionController>().enabled = false;
        _controlChampion.GetComponentInChildren<ChampionAnimation>().ChangeState(ChampionAnimation.AnimState.Idle);
		_controlChampion.enabled = false;
	}

    private void OnChampionTakeDamaged(Champion hitChampion, int damage)
    {
        _battleInfoData.totalHit += damage;
		OnChangedBattleInfoData?.Invoke(battleTeamIndexKey, _battleInfoData);
	}

    private void OnChampionAttack(Champion takeDamagedChampion, int damage)
    {
        if (true == takeDamagedChampion.isDead)
        {
			++_battleInfoData.killCount;

            ++takeDamagedChampion.lastHitChampion.pilotBattleComponent._battleInfoData.assistCount;
			takeDamagedChampion.lastHitChampion.pilotBattleComponent.OnChangedBattleInfoData?.Invoke(battleTeamIndexKey, _battleInfoData);
		}

		OnChangedBattleInfoData?.Invoke(battleTeamIndexKey, _battleInfoData);
	}
}
