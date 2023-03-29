using UnityEngine;
using MH_AIFramework;
using System.Collections.Generic;

public class ChampionBT : BehaviourTree
{
	private void Awake()
	{
	}

	new private void Start()
	{
		base.Start();

		SetupNodeHierarchy();
	}

	private void SetupNodeHierarchy()
	{
		// ��Ʈ �ٷ� �Ʒ� �ڽ� ����..
		SelectorNode rootChildNode = new SelectorNode();
		AddNode( rootChildNode, _rootNode );

		// ========================================================================
		// Death ������ �� ������ ��� ���̾��Ű ����..
		// ========================================================================
		SetupDeadStateNodeHierarchy( rootChildNode );


		// ========================================================================
		// ���� ������ �� ������ ��� ���̾��Ű ����..
		// ========================================================================
		SetupBattleNodeHierarchy( rootChildNode );
	}

	private void SetupDeadStateNodeHierarchy(Node parentNode)
	{
		// Dead State �ֻ��� ��� ���� �� ���..
		SelectorNode deadStateBodyNode = new SelectorNode();
		AddNode( deadStateBodyNode, parentNode );

		// �׾����� üũ�ϴ� ��� ���� �� ���..
		DN_CheckBoolValue checkDeathNode = new DN_CheckBoolValue(true, "isDeath");
		AddNode( checkDeathNode, deadStateBodyNode );

		// �׾��� �� ������ ��� ���� �� ���..

	}

	private void SetupBattleNodeHierarchy(Node parentNode)
	{
		// Battle �ֻ��� ��� ���� �� ���..
		SequenceNode battleBodyNode = new SequenceNode();
		AddNode( battleBodyNode, parentNode );

		// ���� ã�� Service Node ���� �� ���..
		SN_FindTarget findTargetNode = new SN_FindTarget();
		findTargetNode._findTargetFunc = GetComponent<Champion>().FindTarget;
		AddNode( findTargetNode, battleBodyNode );

		// =============================================================================
		// ĳ���Ͱ� ������ �ϱ� ���� �ʿ��� ������ �����ϰ� ���� �ִ��� üũ�ϴ� ��� ���� �� ���..
		SequenceNode targetInfoUpdateParentNode = new SequenceNode();
		AddNode( targetInfoUpdateParentNode, battleBodyNode );

		// ���� ã�Ҵ��� üũ�ϴ� ��� ���� �� ���..
		DN_ValidObjectValue isFindTargetNode = new DN_ValidObjectValue("target");
		AddNode( isFindTargetNode, targetInfoUpdateParentNode );

		// ���� ������ �����ϴ� ��� ���� �� ���..
		AN_UpdateTargetinfomation updateTargetInfoNode = new AN_UpdateTargetinfomation();
		AddNode( updateTargetInfoNode, targetInfoUpdateParentNode );
		// =============================================================================


		// ��Ʋ �ൿ ���� �ֻ��� ��� ���� �� ���..
		SelectorNode battleActionBodyNode = new SelectorNode();
		AddNode( battleActionBodyNode, battleBodyNode );

		// =============================================================================
		// ----- Attack �ൿ ���� ��� ����..
		// ���� �����ϴ� ���� �ֻ��� ��� ���� �� ���..
		SequenceNode attackBodyNode = new SequenceNode();
		AddNode( attackBodyNode, battleActionBodyNode );

		// ���� �����ϱ� ���� ���� ���� ��� ���� �� ���..
		DN_CheckIsCanAttack checkTargetinMyAttackRangeNode = new DN_CheckIsCanAttack();
		AddNode( checkTargetinMyAttackRangeNode, attackBodyNode );
		
		// ���� �����ϴ� ��� ���� �� ���..
		AN_Attack attackActionNode = new AN_Attack();
		AddNode( attackActionNode, attackBodyNode );
		// =============================================================================


		// =============================================================================
		// ----- Chase �ൿ ���� ��� ����..
		// ���� �Ѵ� ���� �ֻ��� ��� ���� �� ���..
		SequenceNode chaseBodyNode = new SequenceNode();
		AddNode( chaseBodyNode, battleActionBodyNode );

		AN_ChaseTarget chaseTargetNode = new AN_ChaseTarget();
		AddNode( chaseTargetNode, chaseBodyNode );
		// =============================================================================
	}
}