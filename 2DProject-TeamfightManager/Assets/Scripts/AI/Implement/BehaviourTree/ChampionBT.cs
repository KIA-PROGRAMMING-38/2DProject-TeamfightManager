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
		blackboard.SetBoolValue(BlackboardKeyTable.IS_DEATH, false);
		blackboard.SetBoolValue(BlackboardKeyTable.IS_ACTION_LOCK, false);
		blackboard.SetBoolValue(BlackboardKeyTable.SPRITE_FLIP_X, false);
		blackboard.SetBoolValue(BlackboardKeyTable.IS_CAN_ACT_ULTIMATE, false);
		blackboard.SetBoolValue(BlackboardKeyTable.IS_ON_KITING, false);

		// float ���� �⺻ ����..
		blackboard.SetFloatValue(BlackboardKeyTable.TARGET_DISTANCE, float.MaxValue);

		// int ���� �⺻ ����..

		// Vector ���� �⺻ ����..
		blackboard.SetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, Vector3.zero);
		blackboard.SetVectorValue(BlackboardKeyTable.EFFECT_DRIECTION, Vector3.zero);

		// Object ���� �⺻ ����..
		blackboard.SetObjectValue(BlackboardKeyTable.TARGET, null);
	}

	private void SetupNodeHierarchy()
	{
		// ��Ʈ �ٷ� �Ʒ� �ڽ� ����..
		SelectorNode rootChildNode = new SelectorNode();
		AddNode(rootChildNode, rootNode, "RootChild");

		// ========================================================================
		// ���� ������ �� ������ ��� ���̾��Ű ����..
		// ========================================================================
		SetupBattleNodeHierarchy(rootChildNode);

		// ���� ã�� ���� ��� Idle �ִϸ��̼����� ������ ���� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Idle, true), rootChildNode, "ChangeAnimState_Idle");
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
		AddNode(new DN_ValidObjectValue(BlackboardKeyTable.TARGET), targetInfoUpdateParentNode);

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
		AddNode(new DN_CheckIsCanAttack(BlackboardKeyTable.IS_CAN_ACT_ULTIMATE, BlackboardKeyTable.ULTIMATE_RANGE), ultimateBodyNode);

		// ���� ���� ���ư��� ���� ���� ���� ��� ���� �� ���..
		AddNode(new AN_LookTarget(), ultimateBodyNode);

		// �ִϸ��̼� ���¸� �ñر� ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Ultimate, true), ultimateBodyNode);

		// �ñر� ��� ��� ���� �� ���..
		AddNode(new AN_Attack(ActionKeyTable.ULTIMATE), ultimateBodyNode);
		// =============================================================================

		// =============================================================================
		// ----- Skill �ൿ ���� ��� ����..
		// ��ų ���� �ֻ��� ��� ���� �� ���..
		Node skillBodyNode = new SequenceNode();
		AddNode(skillBodyNode, battleActionBodyNode);

		// ��ų�� ����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack(BlackboardKeyTable.IS_CAN_ACT_SKILL, BlackboardKeyTable.SKILL_RANGE), skillBodyNode);

		// ���� ���� ���ư��� ���� ���� ���� ��� ���� �� ���..
		AddNode(new AN_LookTarget(), skillBodyNode);

		// �ִϸ��̼� ���¸� ��ų ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Skill, true), skillBodyNode);

		// ��ų ��� ��� ���� �� ���..
		AddNode(new AN_Attack(ActionKeyTable.SKILL), skillBodyNode);
		// =============================================================================

		// =============================================================================
		// ----- Attack �ൿ ���� ��� ����..
		// ���� �����ϴ� ���� �ֻ��� ��� ���� �� ���..
		Node attackBodyNode = new SequenceNode();
		AddNode(attackBodyNode, battleActionBodyNode);

		// ���� �����ϱ� ���� ���� ���� ��� ���� �� ���..
		AddNode(new DN_CheckIsCanAttack(BlackboardKeyTable.IS_CAN_ACT_ATTACK, BlackboardKeyTable.ATTACK_RANGE), attackBodyNode);

		// ���� ���� ���ư��� ���� ���� ���� ��� ���� �� ���..
		AddNode(new AN_LookTarget(), attackBodyNode);

		// �ִϸ��̼� ���¸� ���� ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Attack, true), attackBodyNode);

		// ���� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_Attack(ActionKeyTable.ATTACK), attackBodyNode);
		// =============================================================================


		// =============================================================================
		// ----- Kiting �ൿ ���� ��� ����..
		// ���� �Ѵ� ���� �ֻ��� ��� ���� �� ���..
		Node kitingBodyNode = new SequenceNode();
		AddNode(kitingBodyNode, battleActionBodyNode);

		// ī������ �������� üũ�ϴ� ���� ��� ���� �� ���..
		AddNode(new DN_ConditionKiting(), kitingBodyNode);

		// ī���� ���� �����ϴ� ���� ��� ���� �� ���..
		AddNode(new SN_KitingInfoUpdate(0.2f), kitingBodyNode);

		// �ִϸ��̼� ���¸� ������ ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Move, true), kitingBodyNode);

		// ī���� �������� ������ ��� ���� �� ���..
		AddNode(new AN_Move(), kitingBodyNode);
		// =============================================================================


		// =============================================================================
		// ----- Chase �ൿ ���� ��� ����..
		// ���� �Ѵ� ���� �ֻ��� ��� ���� �� ���..
		Node chaseBodyNode = new SequenceNode();
		AddNode(chaseBodyNode, battleActionBodyNode);

		// ���� �ѱ� ���� ���� ��� ���� �� ���..
		AddNode(new DN_ConditionChaseTarget(), chaseBodyNode);

		// ���� ���� ���ư��� ���� ���� ���� ��� ���� �� ���..
		AddNode(new AN_LookTarget(), chaseBodyNode);

		// �ִϸ��̼� ���¸� ������ ���·� �����ϴ� ��� ���� �� ���..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Move, true), chaseBodyNode);

		// �����̴� ��� ���� �� ���..
		AddNode(new AN_Move(), chaseBodyNode);
		// =============================================================================
	}
}