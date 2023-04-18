using System;
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

            // è�Ǿ��� �̺�Ʈ�� ����..
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

    // �ܺ��� �����ڵ��� ���� �̺�Ʈ��..
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
        // ��ý�Ʈ�� è�Ǿ��� �ִ��� �˻� �� �ִٸ� ���..
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
