using MH_AIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ӿ��� ���� �ο�� è�Ǿ�..
/// AI �� Animation���� �ʿ��� ������ �� è�Ǿ��� �ɷ�ġ ���� è�Ǿ� ���� �������� ���� �� �������ִ� ����..
/// </summary>
public class Champion : MonoBehaviour, IAttackable
{
	public static EffectManager s_effectManager { private get; set; }
	public static DataTableManager s_dataTableManager { private get; set; }

	private ChampionAnimation _animComponent;
	public PilotBattle pilotBattleComponent { get; set; }
	public Blackboard blackboard { get; private set; }

	[field: SerializeField] public ChampionStatus status { get; private set; }
	private ChampionStatus _baseStatus;
	public ChampionData data { get; private set; }
	public ChampionClassType type { get; set; }

	public bool flipX { get => _animComponent.flipX; }

	// ���� ���� ����..
	[SerializeField] private int _curHp = 0;
	public int curHp
	{
		private get => _curHp;
		set
		{
			_curHp = Mathf.Clamp(value, 0, status.hp);

			if (0 == _curHp)
			{
				pilotBattleComponent.OnChampionDead();

				foreach (Effect effect in _activeEffectList)
					effect.Release();

				_activeEffectList.Clear();
			}

			OnChangedHPRatio?.Invoke(curHp / (float)status.hp);
		}
	}
	public bool isDead { get => _curHp == 0; }
	public float speed { get => status.moveSpeed; private set => status.moveSpeed = value; }

	public Champion targetChampion { get => blackboard?.GetObjectValue(BlackboardKeyTable.target) as Champion; }
	public Champion lastHitChampion { get; private set; }

	private AttackAction _attackAction;
	private AttackAction _skillAction;
	private AttackAction _ultimateAction;
	private AttackAction _curAttackAction;

	// ���� ��, �ǰ� �� ����� �̺�Ʈ..
	public event Action<Champion, int> OnHit;       // <������, ������> ������ ����..
	public event Action<Champion, int> OnHill;      // <�� ���ѳ�, ����> ������ ����..
	public event Action<Champion, int> OnAttack;    // <������, ������> ������ ����..
	public event Action<float> OnChangedHPRatio;
	public event Action<float> OnChangedMPRatio;
	public event Action OnUseUltimate;

	// ���� ���� Ÿ��(�� �ʵ��� ���� ��ȿ�ϴٸ� ������ Ÿ���� ���)..
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
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, false);
			}
			else
			{
				StopCoroutine(_updateAtkCooltimeCoroutine);
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, true);
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
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, false);
			}
			else
			{
				OnChangedMPRatio?.Invoke(1f);
				StopCoroutine(_updateSkillCooltimeCoroutine);
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, true);
			}
		}
	}
	private float _skillActTime;

	// ����ϴ� �ڷ�ƾ�� �̸� ĳ��..
	private IEnumerator _onActiveUpdateCoroutine;
	private IEnumerator _updateAtkCooltimeCoroutine;
	private IEnumerator _updateSkillCooltimeCoroutine;
	private IEnumerator _updateModifyStatusSystemCoroutine;

	// ����, ������� ���� ��ȭ�Ǵ� ���� ���� ���� ���� �� ��ȭ�� ������ �����ϴ� ��ü..
	private ChampionModifyStatusSystem _modifyStatusSystem;
	private bool _isRunningModifyStatusSystemLogic;
	private bool isRunningModifyStatusSystemLogic
	{
		get => isRunningModifyStatusSystemLogic;
		set
		{
			if(_isRunningModifyStatusSystemLogic != value)
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

	private void Awake()
	{
		AIController aiController = gameObject.AddComponent<ChampionController>();
		blackboard = GetComponent<AIController>().blackboard;

		if (null == _animComponent)
			_animComponent = GetComponentInChildren<ChampionAnimation>();

		status = new ChampionStatus();

		// Modify Status System �ʱ�ȭ..
		_modifyStatusSystem = new ChampionModifyStatusSystem(this);
		_modifyStatusSystem.OnChangedStatus -= UpdateStatus; 
		_modifyStatusSystem.OnChangedStatus += UpdateStatus;

		// �ڷ�ƾ �Լ� �̸� �ʵ� ��ü�� ����..
		_onActiveUpdateCoroutine = OnActionUpdate();
		_updateAtkCooltimeCoroutine = UpdateAtkCooltime();
		_updateSkillCooltimeCoroutine = UpdateSkillCooltime();
		_updateModifyStatusSystemCoroutine = UpdateModifyStatusSystem();
	}

	private void Start()
	{
		Revival();

		isSkillCooltime = true;
		StartCoroutine(TestUltOn());

		_modifyStatusSystem.effectManager = s_effectManager;
	}

	IEnumerator TestUltOn()
	{
		blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, false);

		yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1f));

		blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, true);
		Debug.Log("�ñر� On");
	}

	private void OnDisable()
	{
		_curAttackAction?.OnAnimationEndEvent();
		_curAttackAction?.OnEnd();
		_curAttackAction = null;

		lastHitChampion = null;

		isRunningModifyStatusSystemLogic = false;
		_modifyStatusSystem?.Reset();
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

	public ChampionStatus debugStatue;
	private void UpdateStatus(ChampionStatus status)
	{
		if (null == _baseStatus)
			return;

		debugStatue = new ChampionStatus
		{
			atkStat = status.atkStat,
			atkSpeed = status.atkSpeed,
			range = status.range,
			defence = status.defence,
			hp = status.hp,
			moveSpeed = status.moveSpeed,
			skillCooltime = status.skillCooltime,
		};

		this.status.atkStat = _baseStatus.atkStat + status.atkStat;
		this.status.atkSpeed = _baseStatus.atkSpeed + status.atkSpeed;
		this.status.range = _baseStatus.range + status.range;
		this.status.defence = _baseStatus.defence + status.defence;
		this.status.hp = _baseStatus.hp + status.hp;
		this.status.moveSpeed = _baseStatus.moveSpeed + status.moveSpeed;
		this.status.skillCooltime = _baseStatus.skillCooltime + status.skillCooltime;
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

	IEnumerator UpdateModifyStatusSystem()
	{
		while(true)
		{
			_modifyStatusSystem.Update();

			if (true == _modifyStatusSystem.isEnded)
			{
				StopCoroutine(_updateModifyStatusSystemCoroutine);
			}

			yield return null;
		}
	}

	// è�Ǿ��� �����ϱ� ���� �ʿ��� �����͸� �޾ƿ� �ʱ�ȭ �ϴ� �Լ�(è�Ǿ� �Ŵ��� Ŭ�������� �Լ��� ȣ���Ѵ�)..
	public void SetupNecessaryData(ChampionStatus status, ChampionData champData, ChampionAnimData animData)
	{
		_baseStatus = status;
		this.data = champData;

		// status �ʱ�ȭ..
		ResetStatus();
		UpdateStatus(this.status);

		// ���� ��ȭ ���� �ý��ۿ��� base status �Ѱ��ֱ�..
		_modifyStatusSystem.championBaseStatus = _baseStatus;

		_animComponent.animData = animData;

		// Data Table���� è�Ǿ� �´� ���� �ൿ�� �����´�..
		_attackAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.atkActionUniqueKey);
		_skillAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.skillActionUniqueKey);
		_ultimateAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.ultimateActionUniqueKey);

		// ���� �ൿ�� ������ <��>�� ����..
		_attackAction.ownerChampion = this;
		_skillAction.ownerChampion = this;
		_ultimateAction.ownerChampion = this;

		_attackAction.effectManager = s_effectManager;
		_skillAction.effectManager = s_effectManager;
		_ultimateAction.effectManager = s_effectManager;

		SetupBlackboard();
	}

	private void SetupBlackboard()
	{
		blackboard.SetFloatValue(BlackboardKeyTable.attackRange, status.range);
		blackboard.SetFloatValue(BlackboardKeyTable.moveSpeed, status.moveSpeed);
	}

	public void Revival()
	{
		curHp = status.hp;

		_animComponent.ResetAnimation();

		isAtkCooltime = false;
		isSkillCooltime = false;
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
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, false);

				OnUseUltimate?.Invoke();
				break;

			default:
				Debug.Assert(false, "Champion's Attack() : invalid atkKind string");
				return;
		}

		blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);
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

				if (_curAttackAction.isEndAction)
				{
					_curAttackAction = null;

					blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, false);

					StopCoroutine(_onActiveUpdateCoroutine);
				}

				yield return null;
			}
			else
			{
				blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, false);

				StopCoroutine(_onActiveUpdateCoroutine);

				yield return null;
			}
		}
	}

	// ���� ��Ÿ�� On(������ ���� �޶��� ���� �ֱ� ������ WaitForSec �� ����)..
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

	// ��ų ��Ÿ�� On(������ ���� �޶��� ���� �ֱ� ������ WaitForSec �� ����)..
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

	// ������ �������� �Ծ��� �� ȣ��Ǵ� �Լ�..
	public void TakeDamage(Champion hitChampion, int damage)
	{
		damage = Math.Min(CalcDefenceApplyDamage(damage), curHp);
		curHp -= damage;

		if (null != hitChampion)
		{
			lastHitChampion = hitChampion;
		}

		OnHit?.Invoke(hitChampion, damage);
		hitChampion.OnAttack?.Invoke(this, damage);

		if (false == isDead)
		{
			_animComponent.OnHit();

			if (null != hitChampion)
			{
				lastHitChampion = hitChampion;
			}
		}
	}

	// ü���� ȸ���� ��(���� ���� �� ���) ȣ��Ǵ� �Լ�..
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

	// �Ѿư� ���� ã�� �Լ�..
	public Champion FindTarget()
	{
#if UNITY_EDITOR
		Debug.Assert(null != pilotBattleComponent);
#endif

		if (null != forcedTarget)
			return forcedTarget;

		return pilotBattleComponent.FindTarget(this);
	}

	// �ִϸ��̼� �̺�Ʈ �� ȣ��Ǵ� �Լ�..
	public void OnAnimEvent(string eventName)
	{
		switch (eventName)
		{
			case "OnAttackAction":
				_curAttackAction?.OnAction();
				break;

			case "OnAnimEnd":
				_curAttackAction?.OnAnimationEndEvent();
				break;
		}
	}

	// ���¿� ���� ������ ���ҵǴ� ����..
	private int CalcDefenceApplyDamage(int damage)
	{
		return (int)((100f / (100 + status.defence)) * damage);
	}
}
