using MH_AIFramework;
using UnityEngine;

/// <summary>
/// è�Ǿ��� �̵� ������ Target�� �ٶ󺸴� �������� �ٲ��ִ� Node..
/// _dirMul�� ���� �ٶ󺸴� ������ ������ �� �ִ�..
/// </summary>
public class AN_LookTarget : ActionNode
{
	private float _dirMul;

	private Transform _transform;

	public AN_LookTarget(float dirMul = 1f)
	{
		_dirMul = dirMul;
	}

	public override void OnCreate()
	{
		base.OnCreate();

		_transform = behaviourTree.gameObject.transform;
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
		//Debug.Log("Ÿ���� �ٶ󺻴�.");
#endif

		Champion target = blackboard.GetObjectValue("target") as Champion;

		if (null == target)
			return State.Failure;

		Vector3 moveDirection = target.transform.position - _transform.position;
		moveDirection = (moveDirection * _dirMul).normalized;

		blackboard.SetVectorValue(BlackboardKeyTable.moveDirection, moveDirection);

#if UNITY_EDITOR
		//Debug.Log("Ÿ���� �ٶ󺸴� �Լ� ��.");
#endif

		return State.Success;
	}
}