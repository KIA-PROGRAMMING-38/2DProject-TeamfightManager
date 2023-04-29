using MH_AIFramework;
using UnityEngine;

/// <summary>
/// 챔피언의 이동 방향을 Target을 바라보는 방향으로 바꿔주는 Node..
/// _dirMul에 따라 바라보는 방향이 조정될 수 있다..
/// </summary>
public class AN_LookTarget : ActionNode
{
	private float _dirMul;

	private Transform _transform;
	private ChampionAnimation _championAnimation;

	public AN_LookTarget(float dirMul = 1f)
	{
		_dirMul = dirMul;
	}

	public override void OnCreate()
	{
		base.OnCreate();

		_transform = behaviourTree.gameObject.transform;
		_championAnimation = behaviourTree.gameObject.GetComponentInChildren<ChampionAnimation>();
	}

	protected override void OnStart()
	{

	}

	protected override void OnStop()
	{

	}

	protected override State OnUpdate()
	{
#if UNITY_EDITOR
		//Debug.Log("타겟을 바라본다.");
#endif

		Champion target = blackboard.GetObjectValue("target") as Champion;

		if (null == target)
			return State.Failure;

		Vector3 moveDirection = target.transform.position - _transform.position;
		moveDirection = (moveDirection * _dirMul).normalized;

		blackboard.SetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, moveDirection);

#if UNITY_EDITOR
		//Debug.Log("타겟을 바라보는 함수 끝.");
#endif

		return State.Success;
	}
}
