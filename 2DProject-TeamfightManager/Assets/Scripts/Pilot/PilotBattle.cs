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

            _controlChampion.OnChangedHPRatio -= UpdateChampionHPRatio;
            _controlChampion.OnChangedHPRatio += UpdateChampionHPRatio;

			_controlChampion.OnChangedMPRatio -= UpdateChampionMPRatio;
			_controlChampion.OnChangedMPRatio += UpdateChampionMPRatio;

			_controlChampion.OnUseUltimate -= UpdateChampionUseUltimateState;
			_controlChampion.OnUseUltimate += UpdateChampionUseUltimateState;
		}
    }
    private Champion _controlChampion;
    public BattleTeam myTeam { get; set; }

    public int battleTeamIndexKey { private get; set; }

    private BattleInfoData _battleInfoData;

    public event Action<int, BattleInfoData> OnChangedBattleInfoData;
    public event Action<int, float> OnChangedChampionHPRatio;
    public event Action<int, float> OnChangedChampionMPRatio;
	public event Action<int> OnChampionUseUltimate;

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
        myTeam.OnChampionDead(champion, battleTeamIndexKey);

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

            Champion assistChampion = takeDamagedChampion.lastHitChampion;
			if (null != assistChampion && this != assistChampion.pilotBattleComponent)
            {
                PilotBattle assistPilotBattle = assistChampion.pilotBattleComponent;
				++assistPilotBattle._battleInfoData.assistCount;

                assistPilotBattle.OnChangedBattleInfoData?.Invoke(assistPilotBattle.battleTeamIndexKey, assistPilotBattle._battleInfoData);
            }
		}

        _battleInfoData.totalDamage += damage;

		OnChangedBattleInfoData?.Invoke(battleTeamIndexKey, _battleInfoData);
	}

    private void UpdateChampionHPRatio(float hpRatio)
    {
        OnChangedChampionHPRatio?.Invoke(battleTeamIndexKey, hpRatio);
    }

	private void UpdateChampionMPRatio(float hpRatio)
	{
		OnChangedChampionMPRatio?.Invoke(battleTeamIndexKey, hpRatio);
	}

    private void UpdateChampionUseUltimateState()
    {
        OnChampionUseUltimate?.Invoke(battleTeamIndexKey);
	}
}
