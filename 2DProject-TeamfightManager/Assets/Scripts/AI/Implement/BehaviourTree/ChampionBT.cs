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
		blackboard.SetBoolValue(BlackboardKeyTable.isDeath, false);
		blackboard.SetBoolValue(BlackboardKeyTable.isActionLock, false);
		blackboard.SetBoolValue(BlackboardKeyTable.spriteFlipX, false);
		blackboard.SetBoolValue(BlackboardKeyTable.isCanActUltimate, false);

		// float ���� �⺻ ����..
		blackboard.SetFloatValue(BlackboardKeyTable.targetDistance, float.MaxValue);

		// int ���� �⺻ ����..

		// Vector ���� �⺻ ����..
		blackboard.SetVectorValue(BlackboardKeyTable.moveDirection, Vector3.zero);

		// Object ���� �⺻ ����..
		blackboard.SetObjectValue(BlackboardKeyTable.target, null);
	}

	private void SetupNodeHierarchy()
	{
		// ��Ʈ �ٷ� �Ʒ� �ڽ� ����..
		SelectorNode rootChildNode = new SelectorNode();
		AddNode(rootChildNode, rootNode);

		// ========================================================================
		// Death ������ �� ������ ��� ���̾��Ű ����..
		// ========================================================================
		//SetupDeadStateNodeHierarchy(rootChildNode);


		// ========================================================================
		// ���� ������ �� ������ ��� ���̾��Ű ����..
		// ========================================================================
		SetupBattleNodeHierarchy(rootChildNode);
	}

	private void SetupDeadStateNodeHierarchy(Node parentNode)
	{
		// Dead State �ֻ��� ��� ���� �� ���..
		Node deadStateBodyNode = new SequenceNode();
		AddNode(deadStateBodyNode, parentNode);

		// �׾����� üũ�ϴ� ��� ���� �� ���..
		AddNode(new DN_CheckBoolValue(true, BlackboardKeyTable.isDeath), deadStateBodyNode);

		// �׾��� �� ������� �ൿ ��� ���� �� ���..
		//AddNode(new AN_OnDeath(), deadStateBodyNode);

		// �׾��� �� ��Ȱ���� ��ٷ����ϴ� �ð��� �ֱ� ������ ��ٸ��� ��� ���� �� ���..
		AddNode(new AN_Wait(1f), deadStateBodyNode);

		// ���� ���� ��Ȱ�� �� è�Ǿ� ��� �ʱ�ȭ�� ��� ���� �� ���..
		//AddNode(new AN_Revival(), deadStateBodyNode);
	}

	private void SetupBattleNodeHierarchy(Node parentNode)
	{
		// Battle �ֻ��� ��� ���� �� ���..
		Node battleBodyNode = new SequenceNode();
		AddNode(battleBodyNode, parentNode);

		// ���� ã�� Service Node ���� �� ���..
		AddNode(new SN_FindTarget(_champion.FindTarget, 0.1f), battleBodyNode);

		// =============================================================================
		// ĳ���Ͱ� ������ �ϱ� ���� �ʿ��� ������ �����ϰ� ���� �ִ��� üũ�ϴ� ��� ���� �� ���..
		Node targetInfoUpdateParentNode = new SequenceNode();
		AddNode(targetInfoUpdateParentNode, battleBodyNode);

		// ���� ã�Ҵ��� üũ�ϴ� ��� ���� �� ���..
		AddNode(new DN_ValidObjectValue(BlackboardKeyTable.target), targetInfoUpdateParentNode);

		// ���� ������ �����ϴ� ��� ���� �� ���..
		AddNode(new AN_UpdateTargetinfomation(), targetInfoUpdateParentNode);
		// =============================================================================


		// ��Ʋ �ൿ ���� �ֻ��� ��� ���� �� ���..
		Node battleActionBodyNode = new SelectorNode();
		AddNode(battleActionBodyNode, battleBodyNode);

		// =============================================================================
		// ----- Ultimate �ൿ ���� ��� ����..
		// ���� �����ϴ� ���� �ֻ��� ��� ���� �� ���..
		Node ultimateBodyNode = new SequenceNode();
		AddNode(ultimateBodyNode, battleActionBodyNode);

		// �ñر⸦ ����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack(BlackboardKeyTable.isCanActUltimate), ultimateBodyNode);

		// �ִϸ��̼� ���¸� �ñر� ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Ultimate, true), ultimateBodyNode);

		// �ñر� ��� ��� ���� �� ���..
		AddNode(new AN_Attack(ActionKeyTable.ultimate), ultimateBodyNode);

		// �ñر� ����Ʈ ���� ��� ���� �� ���..
		AddNode(new AN_ShowEffect(s_effectManager, ActionKeyTable.ultimate), ultimateBodyNode);
		// =============================================================================

		// =============================================================================
		// ----- Skill �ൿ ���� ��� ����..
		// ��ų ���� �ֻ��� ��� ���� �� ���..
		Node skillBodyNode = new SequenceNode();
		AddNode(skillBodyNode, battleActionBodyNode);

		// ��ų�� ����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack(BlackboardKeyTable.isCanActSkill), skillBodyNode);

		// �ִϸ��̼� ���¸� ��ų ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Skill, true), skillBodyNode);

		// ��ų ��� ��� ���� �� ���..
		AddNode(new AN_Attack(ActionKeyTable.skill), skillBodyNode);

		// ��ų ����Ʈ ���� ��� ���� �� ���..
		AddNode(new AN_ShowEffect(s_effectManager, ActionKeyTable.skill), skillBodyNode);
		// =============================================================================

		// =============================================================================
		// ----- Attack �ൿ ���� ��� ����..
		// ���� �����ϴ� ���� �ֻ��� ��� ���� �� ���..
		Node attackBodyNode = new SequenceNode();
		AddNode(attackBodyNode, battleActionBodyNode);

		// ���� �����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack(BlackboardKeyTable.isCanActAttack), attackBodyNode);

		// �ִϸ��̼� ���¸� ���� ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Attack, true), attackBodyNode);

		// ���� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_Attack(ActionKeyTable.attack), attackBodyNode);

		// ���� ����Ʈ ���� ��� ���� �� ���..
		AddNode(new AN_ShowEffect(s_effectManager, ActionKeyTable.attack), attackBodyNode);
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