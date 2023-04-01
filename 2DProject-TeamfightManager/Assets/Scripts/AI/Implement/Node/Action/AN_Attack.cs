using MH_AIFramework;
using UnityEngine;

/// <summary>
/// 챔피언의 공격 행동 Node..
/// </summary>
public class AN_Attack : ActionNode
{
	private IAttackable _attackComponent;
	private string _atkKind;

	public AN_Attack(string atkKind)
	{
		_atkKind = atkKind;
	}

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
		Debug.Log("공격 AI Node 실행됨");
#endif

		_attackComponent.Attack(_atkKind);

		return State.Success;
	}
}
