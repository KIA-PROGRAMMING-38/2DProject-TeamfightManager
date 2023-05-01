using MH_AIFramework;
using System;

public class AN_FindTarget : ActionNode
{
	private Func<Champion> _findTargetFunc;

	public AN_FindTarget(Func<Champion> findTargetFunc)
	{
		_findTargetFunc = findTargetFunc;
	}

	public override void OnCreate()
	{
		base.OnCreate();
	}

	protected override void OnStart()
	{

	}

	protected override void OnStop()
	{

	}

	protected override State OnUpdate()
	{
		Champion findTarget = _findTargetFunc?.Invoke();

		if (null != findTarget)
			blackboard.SetObjectValue(BlackboardKeyTable.TARGET, findTarget);

		return State.Success;
	}
}