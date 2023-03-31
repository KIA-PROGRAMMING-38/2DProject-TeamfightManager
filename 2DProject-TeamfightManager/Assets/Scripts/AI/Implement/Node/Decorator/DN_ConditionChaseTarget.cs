using MH_AIFramework;

/// <summary>
/// 적을 추격할 수 있는지 조건을 검사해주는 Node..
/// </summary>
public class DN_ConditionChaseTarget : DecoratorNode
{
	protected override void OnStart()
	{
		
	}

	protected override void OnStop()
	{
		
	}

	protected override State OnUpdate()
	{
		if (null == blackboard.GetObjectValue("target"))
			return State.Failure;

		if (true == blackboard.GetBoolValue("isMoveLock"))
			return State.Failure;
		
		if ( blackboard.GetFloatValue( "targetDistance" ) <= blackboard.GetFloatValue( "attackRange" ) )
			return State.Failure;

		return State.Success;
	}
}
