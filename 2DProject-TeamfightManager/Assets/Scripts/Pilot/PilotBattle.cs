using System;
using System.Collections;
using System.Collections.Generic;
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

			// 현재 픽한 챔피언의 숙련도가 있는지 체크..
			plusStat = 0;

			List<ChampionSkillLevelInfo> champSkillInfoContainer = pilot.data.champSkillLevelContainer;
			int loopCount = champSkillInfoContainer.Count;
			string champName = controlChampion.data.name;
			for ( int i = 0; i < loopCount; ++i)
			{
				if (champName == champSkillInfoContainer[i].champName)
				{
					plusStat = champSkillInfoContainer[i].level;

					break;
				}
			}

			// 파일럿 능력치 챔피언에게 적용..
			_controlChampion.SetPilotStatus(atkStat, defStat);

			// 챔피언의 이벤트들 구독..
			_controlChampion.OnHit -= OnChampionTakeDamaged;
            _controlChampion.OnHit += OnChampionTakeDamaged;

			_controlChampion.OnKill -= OnChampionKill;
			_controlChampion.OnKill += OnChampionKill;

			_controlChampion.OnHeal -= OnChampionHeal;
			_controlChampion.OnHeal += OnChampionHeal;

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

	// 챔피언 숙련도로 인해 plus 되는 능력치..
	public int plusStat { get; private set; }
	public int atkStat { get => pilot.data.atkStat + plusStat; }
	public int defStat { get => pilot.data.defStat + plusStat; }

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
		float waitTime = UnityEngine.Random.Range(ultWaitTime, ultWaitTime + 10f);
		yield return YieldInstructionStore.GetWaitForSec(waitTime);

		while (true == _controlChampion.isDead)
		{
			yield return null;
		}

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
			// 이벤트 구독 해지..
			DisconectChampEvent(_controlChampion);

			// 챔피언 Release..
			_controlChampion.enabled = true;
			_controlChampion.Release();
			_controlChampion = null;
		}

		// 배틀 정보 초기화..
		_battleInfoData.killCount = 0;
		_battleInfoData.deathCount = 0;
		_battleInfoData.assistCount = 0;
		_battleInfoData.totalAtkDamage = 0;
		_battleInfoData.totalTakeDamage = 0;
		_battleInfoData.totalHeal = 0;

		// 소환수 초기화..
		for (int i = 0; i < _summonChampionContainer.Count; ++i)
		{
			// 이벤트 구독 해지..
			DisconectChampEvent(_summonChampionContainer[i]);

			// 챔피언 Release..
			_summonChampionContainer[i].enabled = true;
			_summonChampionContainer[i].Release();
		}

		_summonChampionContainer.Clear();
		_summonChampionControllerContainer.Clear();

		pilot.Release();
	}

	private void DisconectChampEvent(Champion champion)
	{
		// 챔피언의 이벤트들 구독 해지..
		champion.OnHit -= OnChampionTakeDamaged;
		champion.OnKill -= OnChampionKill;
		champion.OnHeal -= OnChampionHeal;
		champion.OnAttack -= OnChampionAttack;
		champion.OnChangedHPRatio -= UpdateChampionHPRatio;
		champion.OnChangedMPRatio -= UpdateChampionMPRatio;
		champion.OnUseUltimate -= UpdateChampionUseUltimateState;
		champion.OnChangedBarrierRatio -= UpdateBarrierRatio;
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

	private void OnChampionHeal(Champion recoverHPChampion, int healAmount)
	{
		_battleInfoData.totalHeal += healAmount;

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

		// 챔피언의 이벤트들 구독..
		champion.OnKill -= OnChampionKill;
		champion.OnKill += OnChampionKill;

		champion.OnAttack -= OnChampionAttack;
		champion.OnAttack += OnChampionAttack;

		_summonChampionContainer.Add(champion);
		_summonChampionControllerContainer.Add(champion.GetComponent<ChampionController>());

		myTeam.AddActiveChampionList(champion);
	}
}
