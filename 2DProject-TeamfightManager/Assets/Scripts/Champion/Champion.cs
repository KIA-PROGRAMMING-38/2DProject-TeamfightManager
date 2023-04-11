using MH_AIFramework;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 게임에서 실제 싸우는 챔피언..
/// AI 와 Animation에서 필요한 정보들 및 챔피언의 능력치 같은 챔피언 관련 정보들을 저장 및 연결해주는 역할..
/// </summary>
public class Champion : MonoBehaviour, IAttackable
{
	public static EffectManager s_effectManager { private get; set; }
	public static DataTableManager s_dataTableManager { private get; set; }

	private ChampionAnimation _animComponent;
	public PilotBattle pilotBattleComponent { get; set; }
	public Blackboard blackboard { get; private set; }

	public ChampionStatus status { get; private set; }
	public ChampionData data { get; private set; }
	public ChampionClassType type { get; set; }

	public bool flipX { get => _animComponent.flipX; }

	// 현재 상태 관련..
	[SerializeField] private int _curHp = 0;
	public int curHp
	{
		private get => _curHp;
		set
		{
			_curHp = Mathf.Clamp(value, 0, status.hp);

			if (0 == _curHp)
			{
				pilotBattleComponent.OnChampionDead(this);
			}

			OnChangedHPRatio?.Invoke(curHp / (float)status.hp);
		}
	}
	public bool isDead { get => _curHp == 0; }
	public float speed { get => status.moveSpeed; private set => status.moveSpeed = value; }

	public Champion targetChampion { get => blackboard?.GetObjectValue(BlackboardKeyTable.target) as Champion; }
	public Champion lastHitChampion { get; private set; }

	private IEnumerator _atkCooltimeCoroutine;
	private IEnumerator _skillCooltimeCoroutine;

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

	private bool _isAtkCooltime;
	private float _atkActTime;
	private bool isAtkCooltime
	{
		set
		{
			_isAtkCooltime = value;
			if (true == _isAtkCooltime)
				_atkActTime = Time.time;

			blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, !_isAtkCooltime);
		}
	}

	private bool _isSkillCooltime;
	private float _skillActTime;
	private bool isSkillCooltime
	{
		set
		{
			_isSkillCooltime = value;

			if (true == _isSkillCooltime)
				_skillActTime = Time.time;
			else
				OnChangedMPRatio?.Invoke(1f);

			blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, !_isSkillCooltime);
		}
	}

	private IEnumerator _onActiveUpdateCoroutine;

	private void Awake()
	{
		AIController aiController = gameObject.AddComponent<ChampionController>();
		blackboard = GetComponent<AIController>().blackboard;

		if (null == _animComponent)
			_animComponent = GetComponentInChildren<ChampionAnimation>();
	}

	private void Start()
	{
		Revival();

		isSkillCooltime = true;

		//StartCoroutine(TestUltOn());
	}

	private void Update()
	{
		if (true == _isAtkCooltime)
		{
			if (Time.time - _atkActTime >= 1f / status.atkSpeed)
			{
				isAtkCooltime = false;
			}
		}
		if (true == _isSkillCooltime)
		{
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

	IEnumerator TestUltOn()
	{
		blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, false);

		yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1f));

		blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, true);
		Debug.Log("궁극기 On");
	}

	private void OnDisable()
	{
		OnAnimationEnd();
		StopAllCoroutines();

		_curAttackAction?.OnEnd();
		_curAttackAction = null;

		lastHitChampion = null;
	}

	public string ComputeEffectName(string _effectCategory)
	{
		switch (_effectCategory)
		{
			case "Attack":
				return data.atkEffectName;
			case "Skill":
				return data.skillEffectName;
			case "Ultimate":
				return data.ultimateEffectName;
		}

		return "";
	}

	// 챔피언이 동작하기 위해 필요한 데이터를 받아와 초기화 하는 함수(챔피언 매니저 클래스에서 함수를 호출한다)..
	public void SetupNecessaryData(ChampionStatus status, ChampionData champData, ChampionAnimData animData)
	{
		this.status = status;
		this.data = champData;

		_animComponent.animData = animData;

		_attackAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.atkActionUniqueKey);
		_skillAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.skillActionUniqueKey);
		_ultimateAction = s_dataTableManager.attackActionDataTable.GetAttackAction(this.data.ultimateActionUniqueKey);

		_attackAction.ownerChampion = this;
		_skillAction.ownerChampion = this;
		_ultimateAction.ownerChampion = this;

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

	IEnumerator UpdateAtkCooltime()
	{
		while (true)
		{
			Debug.Log("공격 시작");

			yield return YieldInstructionStore.GetWaitForSec(1f / status.atkSpeed);

			blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, true);

			StopCoroutine(_atkCooltimeCoroutine);
		}
	}

	public void Attack(string atkKind)
	{
		//_attackAction.OnAction();
		switch (atkKind)
		{
			case "Attack":
				_curAttackAction = _attackAction;
				_curAttackAction?.OnStart();
				blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, false);
				isAtkCooltime = true;

				break;
			case "Skill":
				_curAttackAction = _skillAction;
				_curAttackAction?.OnStart();
				blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, false);
				isSkillCooltime = true;

				break;
			case "Ultimate":
				_curAttackAction = _ultimateAction;
				_curAttackAction?.OnStart();
				blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, false);

				OnUseUltimate?.Invoke();
				break;

			default:
				Debug.Assert(false, "Champion's Attack() : invalid atkKind string");
				return;
		}

		_onActiveUpdateCoroutine = OnActionUpdate();
		StartCoroutine(_onActiveUpdateCoroutine);
	}

	private IEnumerator OnActionUpdate()
	{
		if (null != _curAttackAction)
		{
			while (true)
			{
				_curAttackAction.OnUpdate();

				if (_curAttackAction.isEndAction)
					break;

				yield return null;
			}

			_curAttackAction = null;
		}

		blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, false);
	}

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

	public Champion FindTarget()
	{
#if UNITY_EDITOR
		Debug.Assert(null != pilotBattleComponent);
#endif

		return pilotBattleComponent.FindTarget(this);
	}

	public void OnAnimEvent(string eventName)
	{
		switch (eventName)
		{
			case "OnAttackAction":
				_curAttackAction?.OnAction();
				break;

			case "OnAnimEnd":
				OnAnimationEnd();
				break;
		}
	}

	private void OnAnimationEnd()
	{
		_curAttackAction?.OnAnimationEndEvent();
	}

	private int CalcDefenceApplyDamage(int damage)
	{
		return (int)((50f / (50 + status.defence)) * damage);
	}

	public void TestColorChange(Color color)
	{
		GetComponentInChildren<SpriteRenderer>().color = color;
	}
}
