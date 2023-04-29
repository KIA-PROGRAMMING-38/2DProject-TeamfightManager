using MH_AIFramework;

public class DN_ConditionKiting : DecoratorNode
{
	private Champion _ownerChampion;

	public override void OnCreate()
	{
		base.OnCreate();

		_ownerChampion = behaviourTree.gameObject.GetComponent<Champion>();
	}

	protected override void OnStart()
	{

	}

	protected override void OnStop()
	{

	}

	protected override State OnUpdate()
	{
		if (false == blackboard.GetBoolValue(BlackboardKeyTable.IS_ON_KITING))
			return State.Failure;

		if (null == blackboard.GetObjectValue(BlackboardKeyTable.TARGET))
			return State.Failure;

		if (true == blackboard.GetBoolValue(BlackboardKeyTable.IS_ACTION_LOCK))
			return State.Failure;

		if (3 == _ownerChampion.data.kitingOrder)
			return State.Failure;

		return State.Success;
	}
}