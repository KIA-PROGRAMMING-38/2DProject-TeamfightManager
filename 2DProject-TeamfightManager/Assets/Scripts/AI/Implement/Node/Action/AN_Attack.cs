using MH_AIFramework;
using UnityEngine;

/// <summary>
/// è�Ǿ��� ���� �ൿ Node..
/// </summary>
public class AN_Attack : ActionNode
{
	private IAttackable _attackComponent;

	public override void OnCreate()
	{
		base.OnCreate();

		_attackComponent = behaviourTree.gameObject.GetComponent<IAttackable>();
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
		Debug.Log("���� AI Node �����");
#endif

		_attackComponent.Attack("Attack");

		return State.Success;
	}
}
