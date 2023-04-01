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

	public string champName { get => "Swordman"; }
	public ChampionStatus status { get; private set; }
	public ChampionClassType type { get; set; }

	public string atkEffectName { get => "Effect_" + champName + "_Attack"; }
	public string skillEffectName { get => "Effect_" + champName + "_Skill"; }

	public bool flipX { get => _animComponent.flipX; }

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

	public float speed { get => status.moveSpeed; private set => status.moveSpeed = value; }

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
		_blackboard.SetFloatValue("attackRange", status.range);
		_blackboard.SetFloatValue("moveSpeed", status.moveSpeed);
	}

	public void Revival()
	{
		curHp = status.hp;

		_animComponent.ResetAnimation();

		_blackboard.SetBoolValue("isCanActSkill", true);
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
					_blackboard.SetBoolValue("isActionLock", true);

					StartCoroutine(UpdateAtkCooltime());
				}
				break;

			case "Skill":
				{
					GameObject target = _blackboard.GetObjectValue("target") as GameObject;

					target.GetComponent<Champion>().Hit(status.atkStat * 2);

					_animComponent.ChangeState(ChampionAnimation.AnimState.Skill);

					_blackboard.SetBoolValue("isCanActSkill", false);
					_blackboard.SetBoolValue("isActionLock", true);

					StartCoroutine(UpdateSkillCoolTime());
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
		_blackboard.SetBoolValue("isActionLock", false);
	}

	public void TestColorChange(Color color)
	{
		GetComponentInChildren<SpriteRenderer>().color = color;
	}
}
