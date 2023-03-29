using MH_AIFramework;
using UnityEngine;

public class AN_UpdateTargetinfomation : ActionNode
{
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

		Vector3 distance = targetObject.transform.position - aiController.transform.position;

		blackboard.SetFloatValue( "targetDistance", distance.magnitude );

		return State.Success;
	}
}
