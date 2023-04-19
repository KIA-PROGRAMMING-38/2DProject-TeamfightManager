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

            // 챔피언의 이벤트들 구독..
            _controlChampion.OnHit -= OnChampionTakeDamaged;
            _controlChampion.OnHit += OnChampionTakeDamaged;

			_controlChampion.OnKill -= OnChampionKill;
			_controlChampion.OnKill += OnChampionKill;

			_controlChampion.OnAttack -= OnChampionAttack;
			_controlChampion.OnAttack += OnChampionAttack;

            _controlChampion.OnChangedHPRatio -= UpdateChampionHPRatio;
            _controlChampion.OnChangedHPRatio += UpdateChampionHPRatio;

			_controlChampion.OnChangedMPRatio -= UpdateChampionMPRatio;
			_controlChampion.OnChangedMPRatio += UpdateChampionMPRatio;

			_controlChampion.OnUseUltimate -= UpdateChampionUseUltimateState;
			_controlChampion.OnUseUltimate += UpdateChampionUseUltimateState;

			_controlChampion.OnChangedBarrierRatio -= UpdateBarrierRatio;
			_controlChampion.OnChangedBarrierRatio += UpdateBarrierRatio;
		}
    }
    private Champion _controlChampion;
    public BattleTeam myTeam { get; set; }

    public int battleTeamIndexKey { private get; set; }

    private BattleInfoData _battleInfoData;

	public int championLayer { get => myTeam.championLayer; }
	public int atkSummonLayer { get => myTeam.atkSummonLayer; }
	public int buffSummonLayer { get => myTeam.buffSummonLayer; }

	public BattlePilotFightData battlePilotFightData
    {
        get
        {
            return new BattlePilotFightData
            {
                pilotName = pilot.data.name,
                championName = "",
                battleData = _battleInfoData
            };
        }
    }

	// 외부의 구독자들을 위한 이벤트들..
	public event Action<int, BattleInfoData> OnChangedBattleInfoData;
    public event Action<int, float> OnChangedChampionHPRatio;
    public event Action<int, float> OnChangedChampionMPRatio;
	public event Action<int> OnChampionUseUltimate;
	public event Action<int, float> OnChangedChampionBarrierRatio;

	private void Awake()
	{
		pilot = GetComponent<Pilot>();
		_battleInfoData = new BattleInfoData();
	}

	public Champion FindTarget(Champion champion)
    {
        return myTeam.ComputeMostNearestEnemyTarget(champion.transform.position);
	}

    public void OnChampionDead()
    {
        myTeam.OnChampionDead(battleTeamIndexKey);

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
        _battleInfoData.totalTakeDamage += damage;
		OnChangedBattleInfoData?.Invoke(battleTeamIndexKey, _battleInfoData);
	}

    private void OnChampionAttack(Champion takeDamagedChampion, int damage)
    {
        _battleInfoData.totalAtkDamage += damage;

		OnChangedBattleInfoData?.Invoke(battleTeamIndexKey, _battleInfoData);
	}

    private void OnChampionKill(Champion killedChampion)
    {
        // 어시스트한 챔피언이 있는지 검사 후 있다면 계산..
		Champion assistChampion = killedChampion.lastHitChampion;
		if (null != assistChampion && this != assistChampion.pilotBattleComponent)
		{
			PilotBattle assistPilotBattle = assistChampion.pilotBattleComponent;
			++assistPilotBattle._battleInfoData.assistCount;

			assistPilotBattle.OnChangedBattleInfoData?.Invoke(assistPilotBattle.battleTeamIndexKey, assistPilotBattle._battleInfoData);
		}

		++_battleInfoData.killCount;
		OnChangedBattleInfoData?.Invoke(battleTeamIndexKey, _battleInfoData);
	}

    private void UpdateChampionHPRatio(float ratio)
    {
        OnChangedChampionHPRatio?.Invoke(battleTeamIndexKey, ratio);
    }

	private void UpdateChampionMPRatio(float ratio)
	{
		OnChangedChampionMPRatio?.Invoke(battleTeamIndexKey, ratio);
	}

    private void UpdateChampionUseUltimateState()
    {
        OnChampionUseUltimate?.Invoke(battleTeamIndexKey);
	}

    private void UpdateBarrierRatio(float ratio)
    {
        OnChangedChampionBarrierRatio?.Invoke(battleTeamIndexKey, ratio);
	}
}
