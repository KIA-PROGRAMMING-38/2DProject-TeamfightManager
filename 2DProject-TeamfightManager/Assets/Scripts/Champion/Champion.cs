using MH_AIFramework;
using System.Collections;
using UnityEngine;

/// <summary>
/// ���ӿ��� ���� �ο�� è�Ǿ�..
/// AI �� Animation���� �ʿ��� ������ �� è�Ǿ��� �ɷ�ġ ���� è�Ǿ� ���� �������� ���� �� �������ִ� ����..
/// </summary>
public class Champion : MonoBehaviour, IAttackable, IHitable
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
				pilotBattleComponent.OnChampionDead(this);
			}
		}
	}
	public bool isDead { get => _curHp == 0; }
	public float speed { get => status.moveSpeed; private set => status.moveSpeed = value; }

	public Champion targetChampion { get => blackboard?.GetObjectValue(BlackboardKeyTable.target) as Champion; }

	private AttackAction _attackAction;
	private AttackAction _skillAction;
	private AttackAction _ultimateAction;
	private AttackAction _curAttackAction;

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

		blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, false);
		StartCoroutine(UpdateSkillCoolTime());

		//StartCoroutine(TestUltOn());
	}

	void OnEnable()
	{
		StartCoroutine(TestUltOn());
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
		OnAnimationEnd();
		StopAllCoroutines();

		_curAttackAction?.OnEnd();
		_curAttackAction = null;
	}

	private void Update()
	{
		//_blackboard?.SetBoolValue(BlackboardKeyTable.isCanActSkill, false);

		//if(null != _blackboard.GetObjectValue(BlackboardKeyTable.target))
		//{
		//	Champion target = _blackboard.GetObjectValue(BlackboardKeyTable.target) as Champion;
		//	if (null != target)
		//		Debug.Log($"{target.name}�� ���� Ÿ��");
		//	else
		//		Debug.Log($"{name}'s target is not null but champion is null");

		//	target = targetChampion;
		//	if (null != target)
		//		Debug.Log($"{target.name}�� ���� Ÿ��2");
		//	else
		//		Debug.Log($"{name}'s target is not null but champion is null2");
		//}
		//else
		//{
		//	Debug.Log($"{name}'s target is null");
		//}
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

		blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, true);
		blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, true);
	}

	IEnumerator UpdateAtkCooltime()
	{
		blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, false);

		yield return YieldInstructionStore.GetWaitForSec(1f / status.atkSpeed);

		blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, true);
	}

	IEnumerator UpdateSkillCoolTime()
	{
		blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, false);

		yield return YieldInstructionStore.GetWaitForSec(status.skillCooltime);

		blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, true);
	}

	public void Attack(string atkKind)
	{
		//_attackAction.OnAction();
		switch(atkKind)
		{
			case "Attack":
				_curAttackAction = _attackAction;
				_curAttackAction?.OnStart();
				blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);
				StartCoroutine(UpdateAtkCooltime());

				break;
			case "Skill":
				_curAttackAction = _skillAction;
				_curAttackAction?.OnStart();
				blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);
				StartCoroutine(UpdateSkillCoolTime());

				break;
			case "Ultimate":
				_curAttackAction = _ultimateAction;
				_curAttackAction?.OnStart();
				blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);
				blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, false);
				break;

			default:
				Debug.Assert(false, "Champion's Attack() : invalid atkKind string");
				return;
		}

		StartCoroutine(OnActionUpdate());
	}

	private IEnumerator OnActionUpdate()
	{
		if(null != _curAttackAction)
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

	public void Hit(int damage)
	{
		curHp -= damage;

		if (false == isDead)
			_animComponent.OnHit();
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

	public void TestColorChange(Color color)
	{
		GetComponentInChildren<SpriteRenderer>().color = color;
	}
}
