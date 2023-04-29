using MH_AIFramework;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SN_KitingInfoUpdate : ServiceNode
{
	private Transform _transform = null;
	private Champion _ownerChampion = null;

	private float _championAttackRange = 0f;
	private float _moveSpeed = 0f;
	private int _championKitingOrder = 0;
	private RaycastHit2D[] _raycastHitCache = null;

	public SN_KitingInfoUpdate(float updateTick)
		: base(updateTick)
	{
		_raycastHitCache = new RaycastHit2D[4];
	}

	public override void OnCreate()
	{
		base.OnCreate();

		_transform = behaviourTree.gameObject.transform;
		_ownerChampion = behaviourTree.gameObject.GetComponent<Champion>();
	}

	protected override void OnStart()
	{
		base.OnStart();

		_championKitingOrder = _ownerChampion.data.kitingOrder;
		_championAttackRange = _ownerChampion.status.range;
		_moveSpeed = _ownerChampion.status.moveSpeed;
	}

	protected override void OnStop()
	{
		base.OnStop();
	}

	protected override State UpdateService()
	{
		Champion target = blackboard.GetObjectValue("target") as Champion;

		if (null == target)
			return State.Failure;

		Vector3 moveDirection = target.transform.position - _transform.position;

		// 사거리 안의 적을 구해 카이팅 order를 비교..
		int enemyCount = Physics2D.RaycastNonAlloc(_ownerChampion.transform.position, moveDirection, _raycastHitCache, _championAttackRange, 1 << target.gameObject.layer);

		for (int i = 0; i < enemyCount; ++i)
		{
			Champion enemy = _raycastHitCache[i].collider.GetComponent<Champion>();
			if (null == enemy)
			{
				continue;
			}

			if (_championKitingOrder <= enemy.data.kitingOrder)
			{
				// 카이팅 방향 계산..
				moveDirection = (_transform.position - enemy.transform.position).normalized;

				moveDirection.x = UnityEngine.Random.Range(moveDirection.x - 0.5f, moveDirection.x + 0.5f);
				moveDirection.y = UnityEngine.Random.Range(moveDirection.y - 0.5f, moveDirection.y + 0.5f);
				moveDirection.z = 0f;

				blackboard.SetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, moveDirection);
				return State.Success;
			}
		}

		blackboard.SetBoolValue(BlackboardKeyTable.IS_ON_KITING, false);

		return State.Failure;
	}
}