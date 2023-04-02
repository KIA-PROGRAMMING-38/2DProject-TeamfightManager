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
		if (null == blackboard.GetObjectValue(BlackboardKeyTable.target))
			return State.Failure;

		if (true == blackboard.GetBoolValue(BlackboardKeyTable.isActionLock))
			return State.Failure;

		if (blackboard.GetFloatValue(BlackboardKeyTable.targetDistance) <= blackboard.GetFloatValue(BlackboardKeyTable.attackRange))
			return State.Failure;

		return State.Success;
	}
}
