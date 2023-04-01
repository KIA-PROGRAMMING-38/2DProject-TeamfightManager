using MH_AIFramework;
using UnityEngine;

/// <summary>
/// è�Ǿ��� �ൿ Ʈ��..
/// è�Ǿ��� �ൿ�� �°� Ʈ�� ������ ��带 �ִ� ���� and �� ������ �˸°� ����..
/// </summary>
public class ChampionBT : BehaviourTree
{
	public static EffectManager s_effectManager { private get; set; }

	private Champion _champion;

	public ChampionBT(AIController aiController) : base(aiController)
	{
		_champion = aiController.GetComponent<Champion>();

		SetupNodeHierarchy();
	}

	public override void OnEnable()
	{
		base.OnEnable();

		SetupBlackboardValue();
	}

	private void SetupBlackboardValue()
	{
		if (null == blackboard)
			return;

		// Bool ���� �⺻ ����..
		blackboard.SetBoolValue("isDeath", false);
		blackboard.SetBoolValue("isActionLock", false);
		blackboard.SetBoolValue("spriteFlipX", false);

		// float ���� �⺻ ����..
		blackboard.SetFloatValue("targetDistance", float.MaxValue);

		// int ���� �⺻ ����..

		// Vector ���� �⺻ ����..
		blackboard.SetVectorValue("moveDirection", Vector3.zero);

		// Object ���� �⺻ ����..
		blackboard.SetObjectValue("target", null);
	}

	private void SetupNodeHierarchy()
	{
		// ��Ʈ �ٷ� �Ʒ� �ڽ� ����..
		SelectorNode rootChildNode = new SelectorNode();
		AddNode(rootChildNode, rootNode);

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
		// ----- Skill �ൿ ���� ��� ����..
		// ���� �����ϴ� ���� �ֻ��� ��� ���� �� ���..
		Node skillBodyNode = new SequenceNode();
		AddNode(skillBodyNode, battleActionBodyNode);

		// ���� �����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack("isCanActSkill"), skillBodyNode);

		// �ִϸ��̼� ���¸� ���� ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Skill, true), skillBodyNode);

		// ���� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_Attack("Skill"), skillBodyNode);

		// ���� ����Ʈ ���� ��� ���� �� ���..
		AddNode(new AN_ShowEffect(s_effectManager, "Skill"), skillBodyNode);
		// =============================================================================

		// =============================================================================
		// ----- Attack �ൿ ���� ��� ����..
		// ���� �����ϴ� ���� �ֻ��� ��� ���� �� ���..
		Node attackBodyNode = new SequenceNode();
		AddNode(attackBodyNode, battleActionBodyNode);

		// ���� �����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack("isAttack"), attackBodyNode);

		// �ִϸ��̼� ���¸� ���� ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Attack, true), attackBodyNode);

		// ���� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_Attack("Attack"), attackBodyNode);

		// ���� ����Ʈ ���� ��� ���� �� ���..
		AddNode(new AN_ShowEffect(s_effectManager, "Attack"), attackBodyNode);
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