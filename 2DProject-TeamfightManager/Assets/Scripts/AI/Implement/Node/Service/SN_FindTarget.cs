using MH_AIFramework;
using System;
using UnityEngine;

/// <summary>
/// 일정 주기마다 적을 찾는 Node..
/// </summary>
public class SN_FindTarget : ServiceNode
{
	private Func<GameObject> _findTargetFunc;

	public SN_FindTarget( Func<GameObject> findTargetFunc, float updateTick)
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
		GameObject findTarget = _findTargetFunc?.Invoke();

		blackboard.SetObjectValue(BlackboardKeyTable.target, findTarget );

		return State.Success;
	}
}
