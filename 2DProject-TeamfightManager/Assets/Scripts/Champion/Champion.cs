using MH_AIFramework;
using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class Champion : MonoBehaviour, IAttackable, IHitable, IWalkable
{
	private ChampionAnimation _animComponent;

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
		return null;
	}

	// ==================================================================================================================
	// --- IWalkable �������̽� �Լ���..
	// ==================================================================================================================
	// ���� è�Ǿ��� �̵���Ű�� �Լ�..
	public void Move(Vector3 direction)
	{
		transform.Translate(Time.deltaTime * speed * direction);

		_animComponent.flipX = direction.x < 0f;
		_animComponent.ChangeState(ChampionAnimation.AnimState.Move);
	}

	// �̵��� ������ �� ����Ǵ� �Լ�(�����̴� �ִϸ��̼� ���߱� ���� �뵵�� ���)..
	public void OnMoveEnd()
	{
		_animComponent.ChangeState(ChampionAnimation.AnimState.Idle);
	}

	public void OnAnimationEnd()
	{
		_blackboard.SetBoolValue("isMoveLock", false);
	}

}
