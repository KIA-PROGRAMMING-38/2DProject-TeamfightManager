using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : SummonObject
{
	public enum ProjectileExecuteImpactType
	{
		OnCollision,
		OnArriveDestination,
		OnArriveTargetPosition,
	}

	[SerializeField] private ProjectileExecuteImpactType _executeImpactType;
	[SerializeField] private bool _isRotateToMoveDir;
	[SerializeField] private float _moveSpeed;

	private Rigidbody2D _rigidbody;

	private Vector2 _destination;
	private Transform _targetTransform;
	private IEnumerator _updateVelToLookTargetCoroutine;
	private IEnumerator _checkIsArriveDestPointCoroutine;

	new private void Awake()
	{
		base.Awake();

		_rigidbody = GetComponent<Rigidbody2D>();

		_updateVelToLookTargetCoroutine = UpdateVelocityToLookTargetDirection();
		_checkIsArriveDestPointCoroutine = CheckIsArriveDestinationPoint();
    }

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		Array.Clear(_targetArray, 0, _targetArray.Length);
	}

	public void SetAdditionalData(int layerMask, Champion target, Func<Vector3, Champion[], int> targetFindFunc)
	{
		gameObject.layer = layerMask;
		_targetFindFunc = targetFindFunc;

		switch (_executeImpactType)
		{
			case ProjectileExecuteImpactType.OnArriveDestination:
				_destination = target.transform.position;
				StartCoroutine(_checkIsArriveDestPointCoroutine);
				break;

			case ProjectileExecuteImpactType.OnArriveTargetPosition:
				_targetTransform = target.transform;
				StartCoroutine(_updateVelToLookTargetCoroutine);
				break;
		}

		UpdateVelocity(target.transform.position);
	}

	private IEnumerator CheckIsArriveDestinationPoint()
	{
		yield return null;

		Vector2 prevPos = Vector2.zero;

		while (true)
		{
			prevPos = _rigidbody.position;

			Debug.Log("??");

			yield return null;

			if (true == IsPassDestinationPoint(prevPos, _rigidbody.position, _destination))
			{
				int findTargetCount = _targetFindFunc.Invoke(_destination, _targetArray);

				ReceiveImpactExecuteEvent(findTargetCount);
				ReceiveReleaseEvent();
				summonObjectManager.ReleaseSummonObject(this);

				yield return null;
			}
		}
	}

	private IEnumerator UpdateVelocityToLookTargetDirection()
	{
		while(true)
		{
			UpdateVelocity(_targetTransform.position);

			yield return null;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("OnTriggerEnter2d");

		if(collision.CompareTag(TagTable.CHAMPION))
		{
            Debug.Log("OnTriggerEnter2d_2");

            Champion target = collision.GetComponent<Champion>();
            if (null != target)
            {
                _targetArray[0] = target;
				ReceiveImpactExecuteEvent(1);
				ReceiveReleaseEvent();
				summonObjectManager.ReleaseSummonObject(this);
            }
        }
	}

	private void UpdateVelocity(Vector3 lookPosition)
	{
		Vector2 direction = lookPosition - transform.position;
		direction.Normalize();

		_rigidbody.velocity = direction * _moveSpeed;
		if (true == _isRotateToMoveDir)
		{
			transform.right = direction;
		}
	}

	// 목표 지점을 지나쳤는지 체크하는 함수..
	private bool IsPassDestinationPoint(Vector2 prevPos, Vector2 curPos, Vector2 destinationPos)
	{
		Vector2 prevPosLookDestPos = destinationPos - prevPos;
		Vector2 curPosLookDestPos = destinationPos - curPos;

		// [이전 방향 -> 도착지] 방향과 [현재 방향 -> 도착지] 방향이 다른 경우 목표 지점을 지나쳤다고 판단..
		float cosTheta = Vector2.Dot(prevPosLookDestPos.normalized, curPosLookDestPos.normalized);
		if (cosTheta < 0.999f)
			return true;

		return false;
	}
}