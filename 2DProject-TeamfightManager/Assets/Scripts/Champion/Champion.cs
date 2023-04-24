using MH_AIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 실제 싸우는 챔피언..
/// AI 와 Animation에서 필요한 정보들 및 챔피언의 능력치 같은 챔피언 관련 정보들을 저장 및 연결해주는 역할..
/// </summary>
public class Champion : MonoBehaviour, IAttackable
{
	public static EffectManager s_effectManager { private get; set; }
	public static DataTableManager s_dataTableManager { private get; set; }
	public static SummonObjectManager s_summonObjeectManager { private get; set; }
	public PilotBattle pilotBattleComponent { get; set; }
	public ChampionManager championManager { private get; set; }

	public Blackboard blackboard { get; private set; }
	private AIController _aiController;
	private ChampionAnimation _animComponent;

	[field: SerializeField] public ChampionStatus status { get; private set; }
	private ChampionStatus _baseStatus;
	public ChampionData data { get; private set; }
	public ChampionClassType type { get; set; }

	public bool flipX { get => _animComponent.flipX; }

	// 현재 상태 관련..
	[SerializeField] private int _curHp = 0;
	public int curHp
	{
		get => _curHp;
		private set
		{
			_curHp = Mathf.Clamp(value, 0, status.hp);

			if (0 == _curHp)
			{
				Death();
			}

			OnChangedHPRatio?.Invoke(hpRatio);
		}
	}
	public float hpRatio { get => curHp / (float)status.hp; }
	public bool isDead { get => _curHp == 0; }
	public float speed { get => status.moveSpeed; private set => status.moveSpeed = value; }

	public Champion targetChampion { get => blackboard?.GetObjectValue(BlackboardKeyTable.TARGET) as Champion; }
	public Champion lastHitChampion { get; private set; }

	private AttackAction _attackAction;
	private AttackAction _skillAction;
	private AttackAction _ultimateAction;
	private AttackAction _curAttackAction;

	// 공격 시, 피격 시 등등의 이벤트..
	public event Action<Champion, int> OnHit;       // <때린놈, 데미지> 순으로 보냄..
	public event Action<Champion, int> OnHill;      // <힐 당한놈, 힐량> 순으로 보냄..
	public event Action<Champion, int> OnAttack;    // <맞은놈, 데미지> 순으로 보냄..
	public event Action<float> OnChangedHPRatio;
	public event Action<float> OnChangedMPRatio;
	public event Action OnUseUltimate;
	public event Action<float> OnChangedBarrierRatio;
	public event Action<Champion> OnKill;
	public event Action<ChampionStatus> OnChangeBuffStatus;

	// 강제 지정 타겟(이 필드의 값이 유효하다면 무조건 타겟은 얘다)..
	private Champion _forcedTarget;
	public Champion forcedTarget
	{
		private get => _forcedTarget;
		set
		{
			_forcedTarget = value;

			if(null != _forcedTarget)
			{
				_curAttackAction?.OnAnimationEndEvent();
				_curAttackAction?.OnEnd();
				_curAttackAction = null;

				_animComponent.ResetAnimation();
			}
		}
	}

	// Attack Cooltime On/Off Logic..
	private bool isAtkCooltime
	{
		set
		{
			if (true == value)
			{
				_atkActTime = Time.time;
				StartCoroutine(_updateAtkCooltimeCoroutine);
				blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_ATTACK, false);
			}
			else
			{
				StopCoroutine(_updateAtkCooltimeCoroutine);
				blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_ATTACK, true);
			}
		}
	}
	private float _atkActTime;

	// Skill Cooltime On/Off Logic..
	private bool isSkillCooltime
	{
		set
		{
			if (true == value)
			{
				_skillActTime = Time.time;
				StartCoroutine(_updateSkillCooltimeCoroutine);
				blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_SKILL, false);
			}
			else
			{
				OnChangedMPRatio?.Invoke(1f);
				StopCoroutine(_updateSkillCooltimeCoroutine);
				blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_SKILL, true);
			}
		}
	}
	private float _skillActTime;

	// 사용하는 코루틴은 미리 캐싱..
	private IEnumerator _onActiveUpdateCoroutine;
	private IEnumerator _updateAtkCooltimeCoroutine;
	private IEnumerator _updateSkillCooltimeCoroutine;
	private IEnumerator _updateModifyStatusSystemCoroutine;

	// 버프, 디버프에 따라 변화되는 스탯 관련 로직 수행 및 변화된 스탯을 제공하는 객체..
	private ChampionModifyStatusSystem _modifyStatusSystem;
	private bool _isRunningModifyStatusSystemLogic;
	private bool isRunningModifyStatusSystemLogic
	{
		get => isRunningModifyStatusSystemLogic;
		set
		{
			if (isDead)
			{
				_isRunningModifyStatusSystemLogic = false;

				return;
			}

			if (_isRunningModifyStatusSystemLogic != value)
			{
				_isRunningModifyStatusSystemLogic = value;

				if (true == _isRunningModifyStatusSystemLogic)
				{
					StartCoroutine(_updateModifyStatusSystemCoroutine);
				}
				else
				{
					StopCoroutine(_updateModifyStatusSystemCoroutine);
				}
			}
		}
	}

	private LinkedList<Effect> _activeEffectList = new LinkedList<Effect>();

	public int championLayer { get => pilotBattleComponent.championLayer; }
	public int atkSummonLayer { get => pilotBattleComponent.atkSummonLayer; }
	public int buffSummonLayer { get => pilotBattleComponent.buffSummonLayer; }
	
	private Collider2D[] _colliders;
	public bool isInvincible
	{
		set
		{
			if (true == value)
			{
				int loopCount = _colliders.Length;
				for( int i = 0; i < loopCount; ++i)
				{
					_colliders[i].enabled = false;
				}

				pilotBattleComponent.RemoveActiveChampionList();
			}
			else
			{
				int loopCount = _colliders.Length;
				for (int i = 0; i < loopCount; ++i)
				{
					_colliders[i].enabled = true;
				}

				pilotBattleComponent.AddActiveChampionList();
			}
		}
	}

	public FindTargetData defaultFindTargetData { get; private set; }

	private void Awake()
	{
		_colliders = GetComponents<Collider2D>();

		_aiController = gameObject.AddComponent<ChampionController>();
		blackboard = _aiController.blackboard;

		if (null == _animComponent)
			_animComponent = GetComponentInChildren<ChampionAnimation>();

		status = new ChampionStatus();

		// Modify Status System 초기화..
		_modifyStatusSystem = new ChampionModifyStatusSystem(this);
		_modifyStatusSystem.OnChangedStatus -= UpdateStatus; 
		_modifyStatusSystem.OnChangedStatus += UpdateStatus;

		_modifyStatusSystem.OnChangedBarrierAmount -= ModifyBarrierAmount;
		_modifyStatusSystem.OnChangedBarrierAmount += ModifyBarrierAmount;
	}

	private void Start()
	{
		_modifyStatusSystem.effectManager = s_effectManager;
    }

	public void Release()
	{
		InitializeData();

		championManager.ReleaseChampion(this);
	}

	IEnumerator TestUltOn()
	{
		blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_ULTIMATE, false);

		yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1f));

		blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_ULTIMATE, true);
		Debug.Log("궁극기 On");
	}

	public void AddActiveEffect(Effect effect)
	{
#if UNITY_EDITOR
		Debug.Assert(null != effect);
#endif

		effect.OnDisableEvent -= OnDisableEffect;

		_activeEffectList.AddLast(effect);
	}

	private void OnDisableEffect(Effect effect)
	{
		_activeEffectList.Remove(effect);
	}

	// 버프, 디버프로 인해 스탯이 변화될 때 호출되는 콜백 함수..
	private void UpdateStatus(ChampionStatus status)
	{
		if (null == _baseStatus)
			return;

		this.status.atkStat = _baseStatus.atkStat + status.atkStat;
		this.status.atkSpeed = _baseStatus.atkSpeed + status.atkSpeed;
		this.status.range = _baseStatus.range + status.range;
		this.status.defence = _baseStatus.defence + status.defence;
		this.status.hp = _baseStatus.hp + status.hp;
		this.status.moveSpeed = _baseStatus.moveSpeed + status.moveSpeed;
		this.status.skillCooltime = _baseStatus.skillCooltime + status.skillCooltime;

		float animSpeed = 1f + (this.status.atkSpeed - _baseStatus.atkSpeed / _baseStatus.atkSpeed) / 100f;
		_animComponent.animationSpeed = animSpeed;

		OnChangeBuffStatus?.Invoke( status );
    }

	private void ResetStatus()
	{
		status.atkStat = 0;
		status.atkSpeed = 0f;
		status.range = 0f;
		status.defence = 0;
		status.hp = 0;
		status.moveSpeed = 0f;
		status.skillCooltime = 0f;
	}

	private void ModifyBarrierAmount(int amount)
	{
		OnChangedBarrierRatio?.Invoke(amount / (float)status.hp);
	}

	IEnumerator UpdateModifyStatusSystem()
	{
		while (true)
		{
			_modifyStatusSystem.Update();

			if (true == _modifyStatusSystem.isEnded)
			{
				StopCoroutine(_updateModifyStatusSystemCoroutine);
			}

			yield return null;
		}
	}

	// 챔피언이 동작하기 위해 필요한 데이터를 받아와 초기화 하는 함수(챔피언 매니저 클래스에서 함수를 호출한다)..
	public void SetupNecessaryData(ChampionStatus status, ChampionData champData, ChampionAnimData animData)
	{
		// 코루틴 함수 미리 필드 객체에 저장..
		_onActiveUpdateCoroutine = OnActionUpdate();
		_updateAtkCooltimeCoroutine = UpdateAtkCooltime();
		_updateSkillCooltimeCoroutine = UpdateSkillCooltime();
		_updateModifyStatusSystemCoroutine = UpdateModifyStatusSystem();

		_baseStatus = status;
		this.data = champData;
		defaultFindTargetData = champData.findTargetData;

		// status 초기화..
		ResetStatus();
		UpdateStatus(this.status);

		// 스탯 변화 로직 시스템에게 base status 넘겨주기..
		_modifyStatusSystem.championBaseStatus = _baseStatus;

		_animComponent.animData = animData;

		// Data Table에서 챔피언에 맞는 공격 행동을 가져온다..
		_attackAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.atkActionUniqueKey);
		_skillAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.skillActionUniqueKey);
		_ultimateAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.ultimateActionUniqueKey);

		// 공격 행동의 주인을 <나>로 설정..
		_attackAction.ownerChampion = this;
		_skillAction.ownerChampion = this;
		_ultimateAction.ownerChampion = this;

		_attackAction.effectManager = s_effectManager;
		_skillAction.effectManager = s_effectManager;
		_ultimateAction.effectManager = s_effectManager;

		_attackAction.summonObjectManager = s_summonObjeectManager;
		_skillAction.summonObjectManager = s_summonObjeectManager;
		_ultimateAction.summonObjectManager = s_summonObjeectManager;

		// Status 관련 데이터 초기화..
		SetupBlackboard();

		_aiController.enabled = true;

		curHp = status.hp;

		isAtkCooltime = false;
		isSkillCooltime = true;

		if (false == s_dataTableManager.attackActionDataTable.GetActionData(this.data.ultimateActionUniqueKey).isPassive)
		{
			//if (this.data.ultimateActionUniqueKey == 23)
			//StartCoroutine(TestUltOn());
		}
	}

	private void Death()
	{
		_animComponent.ChangeState(ChampionAnimation.AnimState.Dead, true);

		pilotBattleComponent.OnChampionDead();

		_aiController.enabled = false;

		InitializeData();
	}

	private void InitializeData()
	{
		foreach (Effect effect in _activeEffectList)
			effect.Release();

		_activeEffectList.Clear();

		_curAttackAction?.OnAnimationEndEvent();
		_curAttackAction?.OnEnd();
		_curAttackAction = null;

		lastHitChampion = null;

		isRunningModifyStatusSystemLogic = false;
		_modifyStatusSystem?.Reset();
	}

	private void SetupBlackboard()
	{
		blackboard.SetBoolValue(BlackboardKeyTable.IS_ACTION_LOCK, false);

		blackboard.SetFloatValue(BlackboardKeyTable.ATTACK_RANGE, status.range);
		blackboard.SetFloatValue(BlackboardKeyTable.MOVE_SPEED, status.moveSpeed);

		blackboard.SetFloatValue(BlackboardKeyTable.SKILL_RANGE, _skillAction.attackRange);
		blackboard.SetFloatValue(BlackboardKeyTable.ULTIMATE_RANGE, _ultimateAction.attackRange);

		Debug.Log($"스킬 사정거리 : {_skillAction.attackRange}");
		Debug.Log($"궁극기 사정거리 : {_ultimateAction.attackRange}");
	}

	public void Revival()
	{
		curHp = status.hp;

		isAtkCooltime = false;
		isSkillCooltime = false;

		_animComponent.ResetAnimation();
		_animComponent.ChangeState(ChampionAnimation.AnimState.Revival);

		_aiController.enabled = true;
	}

	public void Attack(string atkKind)
	{
		switch (atkKind)
		{
			case "Attack":
				_curAttackAction = _attackAction;
				_curAttackAction?.OnStart();
				isAtkCooltime = true;

				break;
			case "Skill":
				_curAttackAction = _skillAction;
				_curAttackAction?.OnStart();
				isSkillCooltime = true;

				break;
			case "Ultimate":
				_curAttackAction = _ultimateAction;
				_curAttackAction?.OnStart();
				blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_ULTIMATE, false);

				OnUseUltimate?.Invoke();
				break;

			default:
				Debug.Assert(false, "Champion's Attack() : invalid atkKind string");
				return;
		}

		blackboard.SetBoolValue(BlackboardKeyTable.IS_ACTION_LOCK, true);
		StartCoroutine(_onActiveUpdateCoroutine);
	}

	// Attack Action Update Logic..
	private IEnumerator OnActionUpdate()
	{
		while(true)
		{
			if (null != _curAttackAction)
			{
				_curAttackAction.OnUpdate();

				if (_curAttackAction.isEnded)
				{
					_curAttackAction?.OnEnd();
					_curAttackAction = null;

					blackboard.SetBoolValue(BlackboardKeyTable.IS_ACTION_LOCK, false);

					StopCoroutine(_onActiveUpdateCoroutine);
				}

				yield return null;
			}
			else
			{
				blackboard.SetBoolValue(BlackboardKeyTable.IS_ACTION_LOCK, false);

				StopCoroutine(_onActiveUpdateCoroutine);

				yield return null;
			}
		}
	}

	// 공격 쿨타임 On(버프에 따라 달라질 수도 있기 때문에 WaitForSec 못 쓸듯)..
	private IEnumerator UpdateAtkCooltime()
	{
		while(true)
		{
			yield return null;

			if (Time.time - _atkActTime >= 1f / status.atkSpeed)
			{
				isAtkCooltime = false;
			}
		}
	}

	// 스킬 쿨타임 On(버프에 따라 달라질 수도 있기 때문에 WaitForSec 못 쓸듯)..
	private IEnumerator UpdateSkillCooltime()
	{
		while (true)
		{
			yield return null;

			float elaspedTime = Time.time - _skillActTime;
			if (elaspedTime >= status.skillCooltime)
			{
				isSkillCooltime = false;
			}
			else
			{
				OnChangedMPRatio?.Invoke(elaspedTime / status.skillCooltime);
			}
		}
	}

	// 적에게 데미지를 입었을 때 호출되는 함수..
	public void TakeDamage(Champion hitChampion, int damage)
	{
		if (true == isDead)
			return;

		// 먼저 방어막이 있다면 방어막부터 깐다..
		damage = _modifyStatusSystem.TakeDamage(damage);

		if (damage > 0)
		{
			damage = Math.Min(CalcDefenceApplyDamage(damage), curHp);
			curHp -= damage;

			OnHit?.Invoke(hitChampion, damage);

			if (false == isDead)
			{
				_animComponent.OnHit();
			}
			else
				hitChampion.OnKill?.Invoke(this);
		}

		hitChampion.OnAttack?.Invoke(this, damage);

		if (null != hitChampion)
		{
			lastHitChampion = hitChampion;
		}
	}

	// 체력이 회복될 때(힐을 받을 때 등등) 호출되는 함수..
	public void RecoverHP(Champion hilledChampion, int hillAmount)
	{
		hillAmount = _modifyStatusSystem.CalcHillDebuff(hillAmount);

		curHp += hillAmount;

		hilledChampion.OnHill?.Invoke(this, hillAmount);
	}

	public void AddBuff(AttackImpactMainData impactMainData, Champion didChampion)
	{
		_modifyStatusSystem.AddBuff((BuffImpactType)impactMainData.detailKind, didChampion, impactMainData.amount, impactMainData.duration);

		if (false == isDead)
			isRunningModifyStatusSystemLogic = true;
	}

	public void AddDebuff(AttackImpactMainData impactMainData, Champion didChampion)
	{
		_modifyStatusSystem.AddDebuff((DebuffImpactType)impactMainData.detailKind, didChampion, impactMainData.amount, impactMainData.duration);

		if (false == isDead)
			isRunningModifyStatusSystemLogic = true;
	}

	// 쫓아갈 적을 찾는 함수..
	public Champion FindTarget()
	{
#if UNITY_EDITOR
		Debug.Assert(null != pilotBattleComponent);
#endif

		if (null != forcedTarget)
			return forcedTarget;

		return pilotBattleComponent.FindTarget(this);
	}

	// 애니메이션 이벤트 시 호출되는 함수..
	public void OnAnimEvent(string eventName)
	{
		switch (eventName)
		{
            case "OnAttackEffectAction":
                _curAttackAction?.OnEffectAction();
                break;

            case "OnAttackPerformanceAction":
                _curAttackAction?.OnPerformanceAction();
                break;

            case "OnAttackImpactAction":
                _curAttackAction?.OnImpactAction();
                break;

			case "OnAttackEnd":
				_curAttackAction?.OnEnd();
				break;

            case "OnAnimEnd":
				_curAttackAction?.OnAnimationEndEvent();
				break;

			case "OnDeathAnimEnd":
				gameObject.SetActive(false);
				break;
		}
	}

	public void OnShowEffectAnimEvent(string effectName)
	{
		s_effectManager.ShowEffect(effectName, transform.position, flipX);
	}

	// 방어력에 따라 데미지 감소되는 로직..
	private int CalcDefenceApplyDamage(int damage)
	{
		return (int)(100f / (100 + status.defence) * damage);
	}
}
