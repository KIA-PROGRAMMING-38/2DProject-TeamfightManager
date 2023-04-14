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
		if (null == blackboard.GetObjectValue(BlackboardKeyTable.TARGET))
			return State.Failure;

		if (true == blackboard.GetBoolValue(BlackboardKeyTable.IS_ACTION_LOCK))
			return State.Failure;

		if (blackboard.GetFloatValue(BlackboardKeyTable.TARGET_DISTANCE) <= blackboard.GetFloatValue(BlackboardKeyTable.ATTACK_RANGE))
			return State.Failure;

		return State.Success;
	}
}
