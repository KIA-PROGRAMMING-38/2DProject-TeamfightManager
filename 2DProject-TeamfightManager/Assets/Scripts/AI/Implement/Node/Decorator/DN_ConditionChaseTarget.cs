using MH_AIFramework;
using UnityEngine;

/// <summary>
/// ���� �߰��� �� �ִ��� ������ �˻����ִ� Node..
/// </summary>
public class DN_ConditionChaseTarget : DecoratorNode
{
	private Vector3 _zeroVector = new Vector3(0f, 0f, 0f);

	public DN_ConditionChaseTarget(Node node)
		: base(node)
	{

	}

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
		{
			blackboard.SetBoolValue(BlackboardKeyTable.IS_ON_KITING, true);
			blackboard.SetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, _zeroVector);
			return State.Failure;
		}

		return base.OnUpdate();
	}
}
