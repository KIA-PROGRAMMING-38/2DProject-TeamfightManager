using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public event Action<Projectile, Champion[], int> OnExecuteImpact;

	public enum ProjectileExecuteImpactType
	{
		OnCollision,
		OnArriveDestination,
		OnArriveTargetPosition,
	}

	public ProjectileManager projectileManager { private get; set; }

	public string projectileName { get => _projectileName; }
	[SerializeField] private string _projectileName;
	[SerializeField] private ProjectileExecuteImpactType _executeImpactType;
	[SerializeField] private bool _isRotateToMoveDir;
	[SerializeField] private float _moveSpeed;

	private Champion[] _targetArray;
	private Func<Vector3, int> _targetFindFunc;

	private Rigidbody2D _rigidbody;

	private Vector2 _destination;
	private Transform _targetTransform;
	private IEnumerator _updateVelToLookTargetCoroutine;
	private IEnumerator _checkIsArriveDestPointCoroutine;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();

		_updateVelToLookTargetCoroutine = UpdateVelocityToLookTargetDirection();
		_checkIsArriveDestPointCoroutine = CheckIsArriveDestinationPoint();

		_targetArray = new Champion[10];
    }

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		Array.Clear(_targetArray, 0, _targetArray.Length);
	}

	public void SetAdditionalData(int layerMask, Champion target, Func<Vector3, int> targetFindFunc)
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
		Vector2 prevPos = Vector2.zero;

		while (true)
		{
			prevPos = _rigidbody.position;

			yield return null;

			if (true == IsPassDestinationPoint(prevPos, _rigidbody.position, _destination))
			{
				int findTargetCount = _targetFindFunc.Invoke(_destination);

				OnExecuteImpact?.Invoke(this, _targetArray, findTargetCount);

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
                OnExecuteImpact?.Invoke(this, _targetArray, 1);

                projectileManager.ReleaseProjectile(this);
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

		// 목표 지점을 지나쳤는지 판단하는 로직..
		// 두 방향의 x,y 중 이전 방향 값은 0이 아닌데 현재 방향 값은 0이라면 지나쳤다고 판단..
		if ((0f != prevPosLookDestPos.x && 0f == curPosLookDestPos.x) ||
			(0f != prevPosLookDestPos.y && 0f == curPosLookDestPos.y))
			return true;

		// 목표 지점을 지나쳤다면 두 방향의 부호가 다르기 때문에 곱셈을 하면 음수가 나옴..
		if ((prevPosLookDestPos.x * curPosLookDestPos.x < 0f) || (prevPosLookDestPos.y * curPosLookDestPos.y < 0f))
			return true;

		return false;
	}
}