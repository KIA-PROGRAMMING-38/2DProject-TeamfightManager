using MH_AIFramework;

/// <summary>
/// ���� �߰��� �� �ִ��� ������ �˻����ִ� Node..
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

		if (true == blackboard.GetBoolValue("isActionLock"))
			return State.Failure;
		
		if ( blackboard.GetFloatValue( "targetDistance" ) <= blackboard.GetFloatValue( "attackRange" ) )
			return State.Failure;

		return State.Success;
	}
}
