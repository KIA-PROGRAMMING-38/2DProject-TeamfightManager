using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonStructure_SwordAura : SummonStructure
{
	private Rigidbody2D _rigidbody;
	[SerializeField] private float _moveSpeed;

	new private void Awake()
	{
		base.Awake();

		_rigidbody = GetComponent<Rigidbody2D>();
	}

	private void OnDisable()
	{
		transform.rotation = Quaternion.identity;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Champion champion = collision.GetComponent<Champion>();
		if(null != champion)
		{
			_targetArray[0] = champion;
			ReceiveImpactExecuteEvent(1);
		}
	}

	public override void SetAdditionalData(int layerMask, Champion target, Champion owner, Func<Vector3, Champion[], int> targetFindFunc)
	{
		base.SetAdditionalData(layerMask, target, owner, targetFindFunc);

		gameObject.layer = LayerTable.Number.CalcOtherTeamLayer(layerMask);

		Vector3 moveDirection = (target.transform.position - transform.position).normalized;
		if (moveDirection.x < 0f)
		{
			float theta = MathF.Acos(Vector3.Dot(Vector3.left, moveDirection)) * Mathf.Rad2Deg;
			if (moveDirection.y < 0f)
				theta *= -1f;

			transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, theta));
		}
		else
		{
			transform.right = moveDirection;
		}
		_rigidbody.velocity = moveDirection * _moveSpeed;
	}
}
