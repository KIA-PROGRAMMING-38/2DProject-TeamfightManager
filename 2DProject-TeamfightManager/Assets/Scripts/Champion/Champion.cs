using MH_AIFramework;
using System.Collections;
using UnityEngine;

public class Champion : MonoBehaviour, IAttackable, IHitable
{
	public static EffectManager s_effectManager { private get; set; }

	private ChampionAnimation _animComponent;
	public PilotBattle pilotBattleComponent { private get; set; }
	private Blackboard _blackboard;

	public string champName { get => "Swordman"; }
	public ChampionStatus status { get; set; }
	public ChampionClassType type { get; set; }
	public ChampionAnimData animData
	{
		set
		{
			if(null == _animComponent)
				_animComponent = GetComponentInChildren<ChampionAnimation>();

			_animComponent.animData = value;
		}
	}

	public string atkEffectName { get => "Effect_" + champName + "_Attack"; }

	public bool flipX
	{
		get => _animComponent.flipX;
	}

	[SerializeField] private int _curHp = 0;

	public int curHp
	{
		private get => _curHp;
		set
		{
			_curHp = Mathf.Clamp(value, 0, status.hp);

			if (0 == value)
			{
				_curHp = 0;
				pilotBattleComponent.OnChampionDead(this);
			}
		}
	}

	public bool isDead { get => _curHp == 0; }

	public float speed
	{
		get
		{
			return status.moveSpeed;
		}
		set
		{
			status.moveSpeed = value;
		}
	}

	private void Awake()
	{
		if (null == _animComponent)
			_animComponent = GetComponentInChildren<ChampionAnimation>();
	}

	private void Start()
	{
		_blackboard = GetComponent<Blackboard>();

		_blackboard.SetFloatValue("attackRange", status.range);
		_blackboard.SetFloatValue("moveSpeed", status.moveSpeed);

		Revival();
	}

	public void Revival()
	{
		curHp = status.hp;

		_animComponent.ResetAnimation();
	}

	IEnumerator UpdateAtkCooltime()
	{
		yield return YieldInstructionStore.GetWaitForSec(status.atkSpeed);

		_blackboard.SetBoolValue("isAttack", true);
	}

	IEnumerator UpdateSkillCoolTime()
	{
		yield return YieldInstructionStore.GetWaitForSec(status.atkSpeed * 2);

		_blackboard.SetBoolValue("isCanActSkill", true);
	}

	public void Attack(string atkKind)
	{
		//_attackAction.OnAction();
		switch(atkKind)
		{
			case "Attack":
				{
					GameObject target = _blackboard.GetObjectValue("target") as GameObject;

					target.GetComponent<Champion>().Hit(status.atkStat);

					_animComponent.ChangeState(ChampionAnimation.AnimState.Attack);

					_blackboard.SetBoolValue("isAttack", false);
					_blackboard.SetBoolValue("isMoveLock", true);

					StartCoroutine(UpdateAtkCooltime());

					s_effectManager.ShowEffect("Effect_Swordman_Attack", transform.position, flipX);
				}
				break;

			case "SKill":
				{
					GameObject target = _blackboard.GetObjectValue("target") as GameObject;

					target.GetComponent<Champion>().Hit(status.atkStat * 2);

					_animComponent.ChangeState(ChampionAnimation.AnimState.Skill);

					_blackboard.SetBoolValue("isCanActSkill", false);
					_blackboard.SetBoolValue("isMoveLock", true);

					StartCoroutine(UpdateAtkCooltime());
				}
				break;
		}
	}

	public void Hit(int damage)
	{
		curHp -= damage;
	}

	public GameObject FindTarget()
	{
#if UNITY_EDITOR
		Debug.Assert(null != pilotBattleComponent);
#endif

		return pilotBattleComponent.FindTarget(this);
	}

	public void OnAnimationEnd()
	{
		_blackboard.SetBoolValue("isMoveLock", false);
	}

	public void TestColorChange(Color color)
	{
		GetComponentInChildren<SpriteRenderer>().color = color;
	}
}
