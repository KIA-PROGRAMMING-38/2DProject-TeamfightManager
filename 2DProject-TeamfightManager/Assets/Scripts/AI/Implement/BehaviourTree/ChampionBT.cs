using UnityEngine;
using MH_AIFramework;

public class ChampionBT : BehaviourTree
{
	public static EffectManager s_effectManager { private get; set; }

	private Champion _champion;

	new private void Awake()
	{
		base.Awake();

		_champion = aiController.GetComponent<Champion>();
	}

	new private void OnEnable()
	{
		base.OnEnable();

		SetupBlackboardValue();
	}

	new private void Start()
	{
		base.Start();

		SetupNodeHierarchy();
	}

	private void SetupBlackboardValue()
	{
		if (null == _blackboard)
			return;

		// Bool ���� �⺻ ����..
		_blackboard.SetBoolValue("isAttack", true);
		_blackboard.SetBoolValue("isDeath", false);
		_blackboard.SetBoolValue("isMoveLock", false);
		_blackboard.SetBoolValue("spriteFlipX", false);

		// float ���� �⺻ ����..
		_blackboard.SetFloatValue("targetDistance", float.MaxValue);

		// int ���� �⺻ ����..

		// Vector ���� �⺻ ����..
		_blackboard.SetVectorValue("moveDirection", Vector3.zero);

		// Object ���� �⺻ ����..
		_blackboard.SetObjectValue("target", null);
	}

	private void SetupNodeHierarchy()
	{
		// ��Ʈ �ٷ� �Ʒ� �ڽ� ����..
		SelectorNode rootChildNode = new SelectorNode();
		AddNode(rootChildNode, _rootNode);

		// ========================================================================
		// Death ������ �� ������ ��� ���̾��Ű ����..
		// ========================================================================
		SetupDeadStateNodeHierarchy(rootChildNode);


		// ========================================================================
		// ���� ������ �� ������ ��� ���̾��Ű ����..
		// ========================================================================
		SetupBattleNodeHierarchy(rootChildNode);
	}

	private void SetupDeadStateNodeHierarchy(Node parentNode)
	{
		// Dead State �ֻ��� ��� ���� �� ���..
		SelectorNode deadStateBodyNode = new SelectorNode();
		AddNode(deadStateBodyNode, parentNode);

		// �׾����� üũ�ϴ� ��� ���� �� ���..
		DN_CheckBoolValue checkDeathNode = new DN_CheckBoolValue(true, "isDeath");
		AddNode(checkDeathNode, deadStateBodyNode);

		// �׾��� �� ������ ��� ���� �� ���..

	}

	private void SetupBattleNodeHierarchy(Node parentNode)
	{
		// Battle �ֻ��� ��� ���� �� ���..
		Node battleBodyNode = new SequenceNode();
		AddNode(battleBodyNode, parentNode);

		// ���� ã�� Service Node ���� �� ���..
		AddNode(new SN_FindTarget(_champion.FindTarget), battleBodyNode);

		// =============================================================================
		// ĳ���Ͱ� ������ �ϱ� ���� �ʿ��� ������ �����ϰ� ���� �ִ��� üũ�ϴ� ��� ���� �� ���..
		Node targetInfoUpdateParentNode = new SequenceNode();
		AddNode(targetInfoUpdateParentNode, battleBodyNode);

		// ���� ã�Ҵ��� üũ�ϴ� ��� ���� �� ���..
		AddNode(new DN_ValidObjectValue("target"), targetInfoUpdateParentNode);

		// ���� ������ �����ϴ� ��� ���� �� ���..
		AddNode(new AN_UpdateTargetinfomation(), targetInfoUpdateParentNode);
		// =============================================================================


		// ��Ʋ �ൿ ���� �ֻ��� ��� ���� �� ���..
		Node battleActionBodyNode = new SelectorNode();
		AddNode(battleActionBodyNode, battleBodyNode);

		// =============================================================================
		// ----- Attack �ൿ ���� ��� ����..
		// ���� �����ϴ� ���� �ֻ��� ��� ���� �� ���..
		Node attackBodyNode = new SequenceNode();
		AddNode(attackBodyNode, battleActionBodyNode);

		// ���� �����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack(), attackBodyNode);

		// �ִϸ��̼� ���¸� ���� ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Attack, true), attackBodyNode);

		// ���� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_Attack(), attackBodyNode);

		// ���� ����Ʈ ���� ��� ���� �� ���..
		AddNode(new AN_ShowEffect(s_effectManager), attackBodyNode);
		// =============================================================================


		// =============================================================================
		// ----- Chase �ൿ ���� ��� ����..
		// ���� �Ѵ� ���� �ֻ��� ��� ���� �� ���..
		Node chaseBodyNode = new SequenceNode();
		AddNode(chaseBodyNode, battleActionBodyNode);

		// ���� �ѱ� ���� ���� ��� ���� �� ���..
		AddNode(new DN_ConditionChaseTarget(), chaseBodyNode);

		// �ִϸ��̼� ���¸� ������ ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Move, true), chaseBodyNode);

		// ���� ���� ���ư��� ���� ���� ���� ��� ���� �� ���..
		AddNode(new AN_LookTarget(), chaseBodyNode);

		// �����̴� ��� ���� �� ���..
		AddNode(new AN_Move(), chaseBodyNode);
		// =============================================================================
	}
}