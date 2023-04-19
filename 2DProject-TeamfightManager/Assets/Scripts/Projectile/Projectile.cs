using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public event Action<int> OnExecuteImpact;

	public enum ProjectileExecuteImpactType
	{
		OnCollision,
		OnArriveDestination,
		OnArriveTargetPosition,
	}

	public string projectileName { get => _projectileName; }
	[SerializeField] private string _projectileName;
	[SerializeField] private ProjectileExecuteImpactType _executeImpactType;
	[SerializeField] private bool _isRotateToMoveDir;
	[SerializeField] private float _moveSpeed;

	private Champion[] _targetArray;

	private Rigidbody2D _rigidbody;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		_targetArray = null;
	}

	public void SetAdditionalData(int layerMask, Champion target, Champion[] targetArray)
	{
		gameObject.layer = layerMask;
		_targetArray = targetArray;

		//Vector2 direction = destination - transform.position;
		//direction.Normalize();
		//
		//_rigidbody.velocity = direction * _moveSpeed;
		//if (true == _isRotateToMoveDir)
		//{
		//	transform.right = direction;
		//}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Champion target = collision.GetComponent<Champion>();
		if (null != target)
		{
			_targetArray[0] = target;

			OnExecuteImpact?.Invoke(1);
		}
	}
}