using MH_AIFramework;

/// <summary>
/// ���� �������� üũ���ִ� Node..
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

		// Ÿ�ٰ��� �Ÿ��� ���� ���� �������� �۴ٸ�..
		float dist = blackboard.GetFloatValue(BlackboardKeyTable.TARGET_DISTANCE);
		if (_attackRange < dist)
			return State.Failure;

		// ���� ���� ��Ÿ���̶��..
		if (false == blackboard.GetBoolValue(_checkAtkBBKey))
			return State.Failure;

		return State.Success;
	}
}
