using System;
using System.Collections;
using System.Collections.Generic;
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
			_championController = _controlChampion.GetComponent<ChampionController>();
			_findTargetData = new FindTargetData
            {
                targetDecideKind = _controlChampion.defaultFindTargetData.targetDecideKind,
                targetTeamKind = _controlChampion.defaultFindTargetData.targetTeamKind,
                impactRange = _controlChampion.defaultFindTargetData.impactRange,
                actionStartPointKind = _controlChampion.defaultFindTargetData.actionStartPointKind,
                isIncludeMe = _controlChampion.defaultFindTargetData.isIncludeMe,
                priorityKind = _controlChampion.defaultFindTargetData.priorityKind,
			};

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
	private ChampionController _championController;
    private FindTargetData _findTargetData;

	private List<Champion> _summonChampionContainer = new List<Champion>();
	private List<ChampionController> _summonChampionControllerContainer = new List<ChampionController>();

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
        return myTeam.FindTarget(champion, _findTargetData);
	}

    public void OnChampionDead(Champion champion)
    {
		if(champion == controlChampion)
		{
			myTeam.OnChampionDead(battleTeamIndexKey);

			++_battleInfoData.deathCount;
		}
		else
		{
			for( int i = 0; i < _summonChampionContainer.Count; ++i)
			{
				if(champion == _summonChampionContainer[i])
				{
                    myTeam.RemoveActiveChampionList(_summonChampionContainer[i]);

                    _summonChampionContainer[i].Release();
					_summonChampionContainer.RemoveAt(i);
					_summonChampionControllerContainer.RemoveAt(i);

					break;
				}
			}
		}
    }

	public void StartBattle(float ultWaitTime)
	{
		StartCoroutine(WaitUltiDelay(ultWaitTime));
	}

	IEnumerator WaitUltiDelay(float ultWaitTime)
	{
		//float waitTime = UnityEngine.Random.Range(ultWaitTime - 5f, ultWaitTime + 5f);
		//yield return YieldInstructionStore.GetWaitForSec(waitTime);
		yield return null;

		_controlChampion.TurnOnUltimate();
	}

    public void StopChampionLogic()
    {
        if (null == _controlChampion)
            return;

		_championController.enabled = false;
        _controlChampion.GetComponentInChildren<ChampionAnimation>().ChangeState(ChampionAnimation.AnimState.Idle, true);
		_controlChampion.enabled = false;

		for( int i = 0; i < _summonChampionContainer.Count; ++i)
		{
			_summonChampionContainer[i].enabled = false;
			_summonChampionContainer[i].GetComponentInChildren<ChampionAnimation>().ChangeState(ChampionAnimation.AnimState.Idle, true);
			_summonChampionControllerContainer[i].enabled = false;
		}
	}

    public void Release()
    {
        if(null != _controlChampion)
        {
			_controlChampion.enabled = true;

			_controlChampion.Release();

			_controlChampion = null;
		}

		_battleInfoData.killCount = 0;
		_battleInfoData.deathCount = 0;
		_battleInfoData.assistCount = 0;
		_battleInfoData.totalAtkDamage = 0;
		_battleInfoData.totalTakeDamage = 0;
		_battleInfoData.totalHeal = 0;

		pilot.Release();

		for (int i = 0; i < _summonChampionContainer.Count; ++i)
		{
			_summonChampionContainer[i].enabled = true;
			_summonChampionContainer[i].Release();
		}

		_summonChampionContainer.Clear();
		_summonChampionControllerContainer.Clear();
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

    public void RemoveActiveChampionList()
    {
        myTeam.RemoveActiveChampionList(controlChampion);
	}

	public void AddActiveChampionList()
	{
        myTeam.AddActiveChampionList(controlChampion);
	}

    public void AddSummonChampion(Champion champion)
    {
		champion.pilotBattleComponent = this;

		// è�Ǿ��� �̺�Ʈ�� ����..
		champion.OnHit -= OnChampionTakeDamaged;
		champion.OnHit += OnChampionTakeDamaged;

		champion.OnKill -= OnChampionKill;
		champion.OnKill += OnChampionKill;

		champion.OnAttack -= OnChampionAttack;
		champion.OnAttack += OnChampionAttack;

		champion.OnChangedHPRatio -= UpdateChampionHPRatio;
		champion.OnChangedHPRatio += UpdateChampionHPRatio;

		champion.OnChangedMPRatio -= UpdateChampionMPRatio;
		champion.OnChangedMPRatio += UpdateChampionMPRatio;

		champion.OnUseUltimate -= UpdateChampionUseUltimateState;
		champion.OnUseUltimate += UpdateChampionUseUltimateState;

		champion.OnChangedBarrierRatio -= UpdateBarrierRatio;
		champion.OnChangedBarrierRatio += UpdateBarrierRatio;

		_summonChampionContainer.Add(champion);
		_summonChampionControllerContainer.Add(champion.GetComponent<ChampionController>());

		myTeam.AddActiveChampionList(champion);
	}
}
