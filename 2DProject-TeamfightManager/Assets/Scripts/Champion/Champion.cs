using MH_AIFramework;
using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class Champion : MonoBehaviour, IAttackable, IHitable, IWalkable
{
	private ChampionAnimation _animComponent;
	public PilotBattle pilotBattleComponent { private get; set; }
	private Blackboard _blackboard;

	public string champName { get; set; }
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

		_blackboard = GetComponent<Blackboard>();
	}

	private void Start()
	{
		_blackboard.SetFloatValue("attackRange", status.range);

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

	// ==================================================================================================================
	// --- IWalkable 인터페이스 함수들..
	// ==================================================================================================================
	// 실제 챔피언을 이동시키는 함수..
	public void Move(Vector3 direction)
	{
		transform.Translate(Time.deltaTime * speed * direction);

		_animComponent.flipX = direction.x < 0f;
		_animComponent.ChangeState(ChampionAnimation.AnimState.Move);
	}

	// 이동이 끝났을 때 실행되는 함수(움직이는 애니메이션 멈추기 등의 용도로 사용)..
	public void OnMoveEnd()
	{
		_animComponent.ChangeState(ChampionAnimation.AnimState.Idle);
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
