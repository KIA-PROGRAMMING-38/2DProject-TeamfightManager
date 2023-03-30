using MH_AIFramework;
using System;
using UnityEngine;

public class SN_FindTarget : ServiceNode
{
	private Func<GameObject> _findTargetFunc;

	public SN_FindTarget( Func<GameObject> findTargetFunc )
		: base( 0.5f )
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

		blackboard.SetObjectValue( "target", findTarget );

		return State.Success;
	}
}
