using MH_AIFramework;
using System;
using System.Collections;
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

	private float _atkActTime;
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

	private float _skillActTime;
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

	private IEnumerator _onActiveUpdateCoroutine;
	private IEnumerator _updateAtkCooltimeCoroutine;
	private IEnumerator _updateSkillCooltimeCoroutine;

	private void Awake()
	{
		AIController aiController = gameObject.AddComponent<ChampionController>();
		blackboard = GetComponent<AIController>().blackboard;

		if (null == _animComponent)
			_animComponent = GetComponentInChildren<ChampionAnimation>();
	}

	private void Start()
	{
		_onActiveUpdateCoroutine = OnActionUpdate();
		_updateAtkCooltimeCoroutine = UpdateAtkCooltime();
		_updateSkillCooltimeCoroutine = UpdateSkillCooltime();

		Revival();

		isSkillCooltime = true;
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

	// è�Ǿ��� �����ϱ� ���� �ʿ��� �����͸� �޾ƿ� �ʱ�ȭ �ϴ� �Լ�(è�Ǿ� �Ŵ��� Ŭ�������� �Լ��� ȣ���Ѵ�)..
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

		StartCoroutine(_onActiveUpdateCoroutine);
	}

	private IEnumerator OnActionUpdate()
	{
		while(true)
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

			StopCoroutine(_onActiveUpdateCoroutine);
			yield return null;
		}
	}

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

	public void TakeDamage(Champion hitChampion, int damage)
	{
		damage = Math.Min(CalcDefenceApplyDamage(damage), curHp) * 5;
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
