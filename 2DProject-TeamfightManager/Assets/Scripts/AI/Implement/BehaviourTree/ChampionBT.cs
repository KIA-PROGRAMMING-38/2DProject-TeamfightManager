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
		// 루트 바로 아래 자식 세팅..
		SelectorNode rootChildNode = new SelectorNode();
		AddNode( rootChildNode, _rootNode );

		// ========================================================================
		// Death 상태일 때 실행할 노드 하이어라키 정의..
		// ========================================================================
		SetupDeadStateNodeHierarchy( rootChildNode );


		// ========================================================================
		// 전투 상태일 때 실행할 노드 하이어라키 정의..
		// ========================================================================
		SetupBattleNodeHierarchy( rootChildNode );
	}

	private void SetupDeadStateNodeHierarchy(Node parentNode)
	{
		// Dead State 최상위 노드 생성 및 등록..
		SelectorNode deadStateBodyNode = new SelectorNode();
		AddNode( deadStateBodyNode, parentNode );

		// 죽었는지 체크하는 노드 생성 및 등록..
		DN_CheckBoolValue checkDeathNode = new DN_CheckBoolValue(true, "isDeath");
		AddNode( checkDeathNode, deadStateBodyNode );

		// 죽었을 때 실행할 노드 생성 및 등록..

	}

	private void SetupBattleNodeHierarchy(Node parentNode)
	{
		// Battle 최상위 노드 생성 및 등록..
		SequenceNode battleBodyNode = new SequenceNode();
		AddNode( battleBodyNode, parentNode );

		// 적을 찾는 Service Node 생성 및 등록..
		SN_FindTarget findTargetNode = new SN_FindTarget();
		findTargetNode._findTargetFunc = GetComponent<Champion>().FindTarget;
		AddNode( findTargetNode, battleBodyNode );

		// =============================================================================
		// 캐릭터가 전투를 하기 위해 필요한 값들을 갱신하고 적이 있는지 체크하는 노드 생성 및 등록..
		SequenceNode targetInfoUpdateParentNode = new SequenceNode();
		AddNode( targetInfoUpdateParentNode, battleBodyNode );

		// 적을 찾았는지 체크하는 노드 생성 및 등록..
		DN_ValidObjectValue isFindTargetNode = new DN_ValidObjectValue("target");
		AddNode( isFindTargetNode, targetInfoUpdateParentNode );

		// 적의 정보를 갱신하는 노드 생성 및 등록..
		AN_UpdateTargetinfomation updateTargetInfoNode = new AN_UpdateTargetinfomation();
		AddNode( updateTargetInfoNode, targetInfoUpdateParentNode );
		// =============================================================================


		// 배틀 행동 관련 최상위 노드 생성 및 등록..
		SelectorNode battleActionBodyNode = new SelectorNode();
		AddNode( battleActionBodyNode, battleBodyNode );

		// =============================================================================
		// ----- Attack 행동 관련 노드 정의..
		// 적을 공격하는 상태 최상위 노드 생성 및 등록..
		SequenceNode attackBodyNode = new SequenceNode();
		AddNode( attackBodyNode, battleActionBodyNode );

		// 적을 공격하기 위한 조건 관련 노드 생성 및 등록..
		DN_CheckIsCanAttack checkTargetinMyAttackRangeNode = new DN_CheckIsCanAttack();
		AddNode( checkTargetinMyAttackRangeNode, attackBodyNode );
		
		// 적을 공격하는 노드 생성 및 등록..
		AN_Attack attackActionNode = new AN_Attack();
		AddNode( attackActionNode, attackBodyNode );
		// =============================================================================


		// =============================================================================
		// ----- Chase 행동 관련 노드 정의..
		// 적을 쫓는 상태 최상위 노드 생성 및 등록..
		SequenceNode chaseBodyNode = new SequenceNode();
		AddNode( chaseBodyNode, battleActionBodyNode );

		AN_ChaseTarget chaseTargetNode = new AN_ChaseTarget();
		AddNode( chaseTargetNode, chaseBodyNode );
		// =============================================================================
	}
}