using MH_AIFramework;
using UnityEngine;

public class AN_ChaseTarget : ActionNode
{
	private IWalkable _walkComponent;
	private Transform _ownerTransform;

	public override void OnCreate()
	{
		base.OnCreate();

		_walkComponent = aiController.GetComponent<IWalkable>();
		_ownerTransform = aiController.transform;
	}

	protected override void OnStart()
	{
		
	}

	protected override void OnStop()
	{
		if( State.Failure == _state )
		{
			_walkComponent.OnMoveEnd();
		}
	}

	protected override State OnUpdate()
	{
#if UNITY_EDITOR
		Debug.Log("Move AI Node ½ÇÇàµÊ");
#endif
		if (true == blackboard.GetBoolValue("isMoveLock"))
			return State.Failure;

		GameObject target = blackboard.GetObjectValue( "target" ) as GameObject;
		if ( null == target )
			return State.Failure;

		if ( blackboard.GetFloatValue( "targetDistance" ) <= blackboard.GetFloatValue( "attackRange" ) )
			return State.Failure;

		Vector3 direction = target.transform.position - _ownerTransform.position;
		_walkComponent.Move(direction.normalized);

#if UNITY_EDITOR
		Debug.Log("Move AI Node ³¡³²");
#endif

		return State.Success;
	}
}
