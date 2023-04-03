using MH_AIFramework;
using System;
using UnityEngine;

/// <summary>
/// ���� �ֱ⸶�� ���� ã�� Node..
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

		blackboard.SetObjectValue(BlackboardKeyTable.target, findTarget );

		return State.Success;
	}
}
