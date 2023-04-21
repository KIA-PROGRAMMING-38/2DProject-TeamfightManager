using MH_AIFramework;

/// <summary>
/// 공격 가능한지 체크해주는 Node..
/// </summary>
public class DN_CheckIsCanAttack : DecoratorNode
{
	private float _attackRange = 0.0f;
	private string _checkAtkBBKey;
	private string _checkRangeBBKey;

	public DN_CheckIsCanAttack(string checkAtkBBKey, string checkRangeBBKey)
	{
		_checkAtkBBKey = checkAtkBBKey;
		_checkRangeBBKey = checkRangeBBKey;
	}

	public override void OnCreate()
	{
		base.OnCreate();
	}

	protected override void OnStart()
	{
		_attackRange = blackboard.GetFloatValue(_checkRangeBBKey);
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

		// 타겟과의 거리가 나의 공격 범위보다 작다면..
		float dist = blackboard.GetFloatValue(BlackboardKeyTable.TARGET_DISTANCE);
		if (_attackRange < dist)
			return State.Failure;

		// 만약 공격 쿨타임이라면..
		if (false == blackboard.GetBoolValue(_checkAtkBBKey))
			return State.Failure;

		return State.Success;
	}
}
