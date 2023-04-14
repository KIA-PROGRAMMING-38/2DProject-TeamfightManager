using UnityEngine;

/// <summary>
/// 퍼포먼스 행동이 움직임 관련인 경우 관련 로직을 제공하는 클래스..
/// </summary>
public class ContPerf_Move : ActionContinuousPerformance
{
	private bool isUseUpdate = false;
	private MovePerformanceType movePerformanceType;

	private Transform targetTransform;
	private Vector3 targetPosition;

	private float _moveSpeed = 0f;

	public ContPerf_Move(AttackAction attackAction, AttackPerformanceData performanceData) : base(attackAction, performanceData)
	{
		movePerformanceType = (MovePerformanceType)performanceData.detailType;

		if (movePerformanceType == MovePerformanceType.MoveToTarget || movePerformanceType == MovePerformanceType.MoveToPosition)
		{
			isUseUpdate = true;
			_moveSpeed = performanceData.floatData[0];
		}
	}

	public override void OnStart()
	{
		targetTransform = ownerChampion.targetChampion.transform;
		if (null != targetTransform)
		{
			targetPosition = targetTransform.position;

			Vector3 dir = (targetPosition - ownerChampion.transform.position).normalized;

			ownerChampion.blackboard.SetVectorValue(BlackboardKeyTable.EFFECT_DRIECTION, dir);
		}

		isEndPerformance = false;
	}

	public override void OnUpdate()
	{
		if (false == isUseUpdate)
			return;
		if (true == isEndPerformance)
			return;
		if (null == ownerChampion || null == ownerChampion.targetChampion)
			return;

		switch (movePerformanceType)
		{
			case MovePerformanceType.MoveToPosition:
				break;
			case MovePerformanceType.MoveToTarget:
				{
					targetPosition = targetTransform.position;
					Vector3 dir = (targetPosition - ownerChampion.transform.position);
					float distance = dir.magnitude;

					if (distance <= ownerChampion.status.range)
					{
						isEndPerformance = true;

						ownerChampion.OnAnimEvent("OnAnimEnd");
					}
					else
					{
						dir /= distance;

						ownerChampion.transform.Translate(Time.deltaTime * _moveSpeed * dir);
					}
				}
				break;
		}

	}

	public override void OnAction()
	{
		if (true == isUseUpdate)
			return;
		if (null == ownerChampion || null == targetTransform)
			return;

		switch(movePerformanceType)
		{
			case MovePerformanceType.TeleportToPosition:
				{
					Vector3 dir = (targetPosition - ownerChampion.transform.position).normalized;
					float distance = performanceData.vectorData[0].z;

					RaycastHit2D hit = Physics2D.Raycast(ownerChampion.transform.position, dir, distance, 1 << LayerMask.NameToLayer("StageOutside"));
					if (null != hit.collider)
					{
						distance = hit.distance;
						Debug.Log("맞음");
					}

					Vector3 destinationPos = ownerChampion.transform.position + dir * distance;
					ownerChampion.transform.position = destinationPos;
				}
				break;

			case MovePerformanceType.TeleportToTarget:
				break;
		}

		isEndPerformance = true;
	}

	public override void OnEnd()
	{
		
	}
}