using MH_AIFramework;
using UnityEngine;

/// <summary>
/// 실제 움직임을 담당하는 Node..
/// </summary>
public class AN_Move : ActionNode
{
	private Transform _transform;

	private float _speed;
	private Vector3 _moveDirection;

	public override void OnCreate()
	{
		base.OnCreate();

		_transform = behaviourTree.gameObject.transform;
	}

	protected override void OnStart()
	{
		_speed = blackboard.GetFloatValue("moveSpeed");
		blackboard.GetVectorValue("moveDirection", out _moveDirection);
		_moveDirection.Normalize();
	}

	protected override void OnStop()
	{
		
	}

	protected override State OnUpdate()
	{
#if UNITY_EDITOR
		Debug.Log("움직인다");
		Debug.Assert(null != _transform);
#endif

		_transform.Translate(Time.deltaTime * _speed * _moveDirection, Space.World);

		//Debug.Log($"{Time.deltaTime * _speed * _moveDirection} 방향으로 움직임");

		return State.Success;
	}
}