using MH_AIFramework;
using UnityEngine;

/// <summary>
/// 타겟과 관련된 정보를 갱신해주는 Node..
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
		GameObject targetObject = blackboard.GetObjectValue( "target" ) as GameObject;
		if ( null == targetObject )
			return State.Failure;

		Vector3 distance = targetObject.transform.position - _transform.transform.position;

		blackboard.SetFloatValue( "targetDistance", distance.magnitude );

		return State.Success;
	}
}
