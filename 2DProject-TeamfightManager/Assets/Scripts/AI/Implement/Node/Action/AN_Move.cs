using MH_AIFramework;
using UnityEngine;
using UnityEngine.EventSystems;

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
		_speed = blackboard.GetFloatValue(BlackboardKeyTable.MOVE_SPEED);
		blackboard.GetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, out _moveDirection);
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

		// 만약 맵 밖으로 벗어나려고 한다면 못 벗어나게 방향을 조정해준다..
		RaycastHit2D hit;
		hit = Physics2D.Raycast(_transform.position, _moveDirection, 0.5f, 1 << LayerTable.Number.STAGE_AREALIMITLINE);
		if (null != hit.collider)
		{
			Vector2 normalVec = CalcNormalVector(hit.collider).normalized;
			_moveDirection.x = normalVec.y * -1f;
			_moveDirection.y = normalVec.x;

			blackboard.SetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, _moveDirection);
		}

		_transform.Translate(Time.deltaTime * _speed * _moveDirection, Space.World);

		return State.Success;
	}

	private Vector2 CalcNormalVector(Collider2D collider)
	{
		return collider.gameObject.transform.position - collider.bounds.center;
	}
}