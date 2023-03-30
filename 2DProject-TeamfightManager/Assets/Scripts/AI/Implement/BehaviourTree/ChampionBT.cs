using UnityEngine;
using MH_AIFramework;

public class ChampionBT : BehaviourTree
{
	public static EffectManager s_effectManager { private get; set; }

	private Champion _champion;

	new private void Awake()
	{
		base.Awake();

		_champion = GetComponent<Champion>();
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
		if (null == blackboard)
			return;

		// Bool 값들 기본 세팅..
		blackboard.SetBoolValue("isAttack", true);
		blackboard.SetBoolValue("isDeath", false);
		blackboard.SetBoolValue("isMoveLock", false);
		blackboard.SetBoolValue("spriteFlipX", false);

		// float 값들 기본 세팅..
		blackboard.SetFloatValue("targetDistance", float.MaxValue);

		// int 값들 기본 세팅..

		// Vector 값들 기본 세팅..
		blackboard.SetVectorValue("moveDirection", Vector3.zero);

		// Object 값들 기본 세팅..
		blackboard.SetObjectValue("target", null);
	}

	private void SetupNodeHierarchy()
	{
		// 루트 바로 아래 자식 세팅..
		SelectorNode rootChildNode = new SelectorNode();
		AddNode(rootChildNode, _rootNode);

		// ========================================================================
		// Death 상태일 때 실행할 노드 하이어라키 정의..
		// ========================================================================
		SetupDeadStateNodeHierarchy(rootChildNode);


		// ========================================================================
		// 전투 상태일 때 실행할 노드 하이어라키 정의..
		// ========================================================================
		SetupBattleNodeHierarchy(rootChildNode);
	}

	private void SetupDeadStateNodeHierarchy(Node parentNode)
	{
		// Dead State 최상위 노드 생성 및 등록..
		SelectorNode deadStateBodyNode = new SelectorNode();
		AddNode(deadStateBodyNode, parentNode);

		// 죽었는지 체크하는 노드 생성 및 등록..
		DN_CheckBoolValue checkDeathNode = new DN_CheckBoolValue(true, "isDeath");
		AddNode(checkDeathNode, deadStateBodyNode);

		// 죽었을 때 실행할 노드 생성 및 등록..

	}

	private void SetupBattleNodeHierarchy(Node parentNode)
	{
		// Battle 최상위 노드 생성 및 등록..
		Node battleBodyNode = new SequenceNode();
		AddNode(battleBodyNode, parentNode);

		// 적을 찾는 Service Node 생성 및 등록..
		AddNode(new SN_FindTarget(_champion.FindTarget), battleBodyNode);

		// =============================================================================
		// 캐릭터가 전투를 하기 위해 필요한 값들을 갱신하고 적이 있는지 체크하는 노드 생성 및 등록..
		Node targetInfoUpdateParentNode = new SequenceNode();
		AddNode(targetInfoUpdateParentNode, battleBodyNode);

		// 적을 찾았는지 체크하는 노드 생성 및 등록..
		AddNode(new DN_ValidObjectValue("target"), targetInfoUpdateParentNode);

		// 적의 정보를 갱신하는 노드 생성 및 등록..
		AddNode(new AN_UpdateTargetinfomation(), targetInfoUpdateParentNode);
		// =============================================================================


		// 배틀 행동 관련 최상위 노드 생성 및 등록..
		Node battleActionBodyNode = new SelectorNode();
		AddNode(battleActionBodyNode, battleBodyNode);

		// =============================================================================
		// ----- Attack 행동 관련 노드 정의..
		// 적을 공격하는 상태 최상위 노드 생성 및 등록..
		Node attackBodyNode = new SequenceNode();
		AddNode(attackBodyNode, battleActionBodyNode);

		// 적을 공격하기 위한 조건 관련 노드 생성 및 등록..
		AddNode(new DN_CheckIsCanAttack(), attackBodyNode);

		// 애니메이션 상태를 공격 상태로 변경하는 노드 생성 및 등록..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Attack, true), attackBodyNode);

		// 적을 공격하는 노드 생성 및 등록..
		AddNode(new AN_Attack(), attackBodyNode);

		// 공격 이펙트 생성 노드 생성 및 등록..
		AddNode(new AN_ShowEffect(s_effectManager), attackBodyNode);
		// =============================================================================


		// =============================================================================
		// ----- Chase 행동 관련 노드 정의..
		// 적을 쫓는 상태 최상위 노드 생성 및 등록..
		Node chaseBodyNode = new SequenceNode();
		AddNode(chaseBodyNode, battleActionBodyNode);

		// 적을 쫓기 위한 조건 노드 생성 및 등록..
		AddNode(new DN_ConditionChaseTarget(), chaseBodyNode);

		// 애니메이션 상태를 움직임 상태로 변경하는 노드 생성 및 등록..
		AddNode(new AN_ChangeAnimState(ChampionAnimation.AnimState.Move, true), chaseBodyNode);

		// 적을 향해 나아가기 위한 방향 설정 노드 생성 및 등록..
		AddNode(new AN_LookTarget(), chaseBodyNode);

		// 움직이는 노드 생성 및 등록..
		AddNode(new AN_Move(), chaseBodyNode);
		// =============================================================================
	}
}