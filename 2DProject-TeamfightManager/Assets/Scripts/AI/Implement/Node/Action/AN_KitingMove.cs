using MH_AIFramework;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;

public class AN_KitingMove : ActionNode
{
	private Transform _transform = null;
	private Champion _ownerChampion = null;

	private float _championAttackRange = 0f;
	private float _moveSpeed = 0f;
	private int _championKitingOrder = 0;
	private RaycastHit2D[] _raycastHitCache = null;

	public AN_KitingMove()
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
		_championKitingOrder = _ownerChampion.data.kitingOrder;
		_championAttackRange = _ownerChampion.status.range;
		_moveSpeed = _ownerChampion.status.moveSpeed;
	}

	protected override void OnStop()
	{

	}

	protected override State OnUpdate()
	{
		Champion target = blackboard.GetObjectValue("target") as Champion;

		if (null == target)
			return State.Failure;

		Vector3 moveDirection = target.transform.position - _transform.position;

		// 사거리 안의 적을 구해 카이팅 order를 비교..
		int enemyCount = Physics2D.RaycastNonAlloc(_ownerChampion.transform.position, moveDirection, _raycastHitCache, _championAttackRange, 1 << target.gameObject.layer);

		bool isOnKiting = false;
		for (int i = 0; i < enemyCount; ++i)
		{
			Champion enemy = _raycastHitCache[i].collider.GetComponent<Champion>();
			if (null == enemy)
			{
				continue;
			}

			if (_championKitingOrder <= enemy.data.kitingOrder)
			{
				moveDirection = (_transform.position - enemy.transform.position).normalized;
				isOnKiting = true;

				break;
			}
		}

		// 카이팅 해야한다면..
		if (true == isOnKiting)
		{
			// 카이팅 방향 계산..
			moveDirection.x = UnityEngine.Random.Range(moveDirection.x - 0.3f, moveDirection.x + 0.3f);
			moveDirection.y = UnityEngine.Random.Range(moveDirection.y - 0.3f, moveDirection.y + 0.3f);
			moveDirection.z = 0f;
			moveDirection.Normalize();

			// 만약 맵 밖으로 벗어나려고 한다면 슬라이딩 벡터를 계산한다..
			RaycastHit2D hit;
			hit = Physics2D.Raycast(_transform.position, moveDirection, 0.5f, 1 << LayerTable.Number.STAGE_AREALIMITLINE);
			if (null != hit.collider)
			{
				Vector2 normalVec = CalcNormalVector(hit.collider).normalized;
				moveDirection.x = normalVec.y * -1f;
				moveDirection.y = normalVec.x;
			}

			moveDirection.Normalize();

			blackboard.SetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, moveDirection);

			_transform.Translate(Time.deltaTime * _moveSpeed * moveDirection, Space.World);

			return State.Success;
		}

		return State.Failure;
	}

	private Vector2 CalcNormalVector(Collider2D collider)
	{
		return collider.gameObject.transform.position - collider.bounds.center;
	}
}
