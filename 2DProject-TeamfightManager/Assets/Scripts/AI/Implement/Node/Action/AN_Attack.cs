using MH_AIFramework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AN_Attack : ActionNode
{
	private IAttackable _attackComponent;

	public override void OnCreate()
	{
		base.OnCreate();

		_attackComponent = aiController.GetComponent<IAttackable>();
	}

	protected override void OnStart()
	{
		
	}

	protected override void OnStop()
	{
		
	}

	protected override State OnUpdate()
	{
#if UNITY_EDITOR
		Debug.Log("°ø°Ý AI Node ½ÇÇàµÊ");
#endif

		_attackComponent.Attack("Attack");

		return State.Success;
	}
}
