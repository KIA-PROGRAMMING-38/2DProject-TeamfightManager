using MH_AIFramework;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// 게임에서 실제 싸우는 챔피언..
/// AI 와 Animation에서 필요한 정보들 및 챔피언의 능력치 같은 챔피언 관련 정보들을 저장 및 연결해주는 역할..
/// </summary>
public class Champion : MonoBehaviour, IAttackable, IHitable
{
	public static EffectManager s_effectManager { private get; set; }

	private ChampionAnimation _animComponent;
	public PilotBattle pilotBattleComponent { private get; set; }
	private Blackboard _blackboard;

	public ChampionStatus status { get; private set; }
	public ChampionClassType type { get; set; }

	public string champName { get => "Swordman"; }
	public string atkEffectName { get => "Effect_" + champName + "_Attack"; }
	public string skillEffectName { get => "Effect_" + champName + "_Skill"; }

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
		}
	}
	public bool isDead { get => _curHp == 0; }
	public float speed { get => status.moveSpeed; private set => status.moveSpeed = value; }

	private AttackAction _attackAction;
	private AttackAction _skillAction;
	private AttackAction _curAttackAction;
	public Champion targetChampion { get => _blackboard?.GetObjectValue(BlackboardKeyTable.target) as Champion; }

	private void Awake()
	{
		AIController aiController = gameObject.AddComponent<ChampionController>();
		_blackboard = GetComponent<AIController>().blackboard;

		if (null == _animComponent)
			_animComponent = GetComponentInChildren<ChampionAnimation>();
	}

	private void Start()
	{
		Revival();

		_blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, false);
		StartCoroutine(UpdateSkillCoolTime());

		_attackAction = new AttackAction(this, new AttackActionData
		{
			isPassive = false,
			uniqueKey = 0,
			impactRange = 0,
			impactRangeType = (int)ImpactRangeKind.OnlyTarget
		});
		_attackAction.AddImpactData(new AttackImpactData
		{
			amount = 10,
			detailKind = (int)AttackImpactType.DefaultAttack,
			duration = 0,
			kind = (int)AttackImpactEffectKind.Attack,
			tickTime = 0,
			targetDecideKind = (int)TargetDecideKind.OnlyTarget
		});

		_skillAction = new AttackAction(this, new AttackActionData
		{
			isPassive = false,
			uniqueKey = 1,
			impactRange = 0,
			impactRangeType = (int)ImpactRangeKind.Range_Circle
		});
		_skillAction.AddImpactData(new AttackImpactData
		{
			amount = 20,
			detailKind = (int)AttackImpactType.DefaultAttack,
			duration = 0,
			kind = (int)AttackImpactEffectKind.Attack,
			tickTime = 0,
			targetDecideKind = (int)TargetDecideKind.InRange
		});
	}

	private void OnDisable()
	{
		OnAnimationEnd();
		StopAllCoroutines();
	}

	private void Update()
	{
		//if(null != _blackboard.GetObjectValue(BlackboardKeyTable.target))
		//{
		//	Champion target = _blackboard.GetObjectValue(BlackboardKeyTable.target) as Champion;
		//	if (null != target)
		//		Debug.Log($"{target.name}은 나의 타겟");
		//	else
		//		Debug.Log($"{name}'s target is not null but champion is null");

		//	target = targetChampion;
		//	if (null != target)
		//		Debug.Log($"{target.name}은 나의 타겟2");
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
				return atkEffectName;
			case "Skill":
				return skillEffectName;
		}

		return "";
	}

	public void SetupNecessaryData(ChampionStatus status, ChampionAnimData animData)
	{
		this.status = status;

		_animComponent.animData = animData;

		SetupBlackboard();
	}

	private void SetupBlackboard()
	{
		_blackboard.SetFloatValue(BlackboardKeyTable.attackRange, status.range);
		_blackboard.SetFloatValue(BlackboardKeyTable.moveSpeed, status.moveSpeed);
	}

	public void Revival()
	{
		curHp = status.hp;

		_animComponent.ResetAnimation();

		_blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, true);
	}

	IEnumerator UpdateAtkCooltime()
	{
		_blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, false);

		yield return YieldInstructionStore.GetWaitForSec(status.atkSpeed);

		_blackboard.SetBoolValue(BlackboardKeyTable.isCanActAttack, true);
	}

	IEnumerator UpdateSkillCoolTime()
	{
		yield return YieldInstructionStore.GetWaitForSec(status.atkSpeed * 2);

		_blackboard.SetBoolValue(BlackboardKeyTable.isCanActSkill, true);
	}

	public void Attack(string atkKind)
	{
		//_attackAction.OnAction();
		switch(atkKind)
		{
			case "Attack":
				_curAttackAction = _attackAction;
				_curAttackAction?.OnStart();
				StartCoroutine(UpdateAtkCooltime());

				break;

			case "Skill":
				_curAttackAction = _skillAction;
				_curAttackAction?.OnStart();
				StartCoroutine(UpdateSkillCoolTime());
				break;

			case "Ultimate":
				{
					GameObject target = _blackboard.GetObjectValue(BlackboardKeyTable.target) as GameObject;

					target.GetComponent<Champion>().Hit(1000);

					_animComponent.ChangeState(ChampionAnimation.AnimState.Ultimate);

					_blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, false);
					_blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, true);

					StartCoroutine(UpdateSkillCoolTime());
				}
				break;
		}
	}

	public void Hit(int damage)
	{
		curHp -= damage;
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
		_curAttackAction?.OnEnd();
		_blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, false);
	}

	public void TestColorChange(Color color)
	{
		GetComponentInChildren<SpriteRenderer>().color = color;
	}
}
