using MH_AIFramework;
using System;
using UnityEngine;

/// <summary>
/// 일정 주기마다 적을 찾는 Node..
/// </summary>
public class SN_FindTarget : ServiceNode
{
	private Func<Champion> _findTargetFunc;

	public SN_FindTarget( Func<Champion> findTargetFunc, float updateTick)
		: base(updateTick)
	{
		_findTargetFunc = findTargetFunc;
	}

	public override void OnCreate()
	{
		base.OnCreate();
	}

	protected override void OnStart()
	{
		base.OnStart();
	}

	protected override void OnStop()
	{
		base.OnStop();
	}

	protected override State UpdateService()
	{
		Champion findTarget = _findTargetFunc?.Invoke();

		if (null != findTarget)
			blackboard.SetObjectValue(BlackboardKeyTable.TARGET, findTarget );

		return State.Success;
	}
}
