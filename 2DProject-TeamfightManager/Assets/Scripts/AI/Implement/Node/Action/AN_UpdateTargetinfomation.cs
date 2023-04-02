using MH_AIFramework;
using UnityEngine;

/// <summary>
/// Ÿ�ٰ� ���õ� ������ �������ִ� Node..
/// </summary>
public class AN_UpdateTargetinfomation : ActionNode
{
	private Transform _transform;

	public override void OnCreate()
	{
		base.OnCreate();

		_transform = behaviourTree.gameObject.transform;
	}

	protected override void OnStart()
	{
		
	}

	protected override void OnStop()
	{
		
	}

	protected override State OnUpdate()
	{
		GameObject targetObject = blackboard.GetObjectValue(BlackboardKeyTable.target) as GameObject;
		if ( null == targetObject )
			return State.Failure;

		Vector3 distance = targetObject.transform.position - _transform.transform.position;

		blackboard.SetFloatValue(BlackboardKeyTable.targetDistance, distance.magnitude);

		return State.Success;
	}
}
