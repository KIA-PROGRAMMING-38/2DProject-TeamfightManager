using UnityEngine;

/// <summary>
/// 퍼포먼스 행동이 움직임 관련인 경우 관련 로직을 제공하는 클래스..
/// </summary>
public class ContPerf_Move : ActionContinuousPerformance
{
	private bool _isUseUpdate = false;
	private MovePerformanceType _movePerformanceType;

	private bool _isOnAction = false;

	private Transform _targetTransform;
	private Vector3 _targetPosition;

	private float _moveSpeed = 0f;

	public ContPerf_Move(AttackAction attackAction, AttackPerformanceData performanceData) : base(attackAction, performanceData)
	{
		_movePerformanceType = (MovePerformanceType)performanceData.detailType;

		if (_movePerformanceType == MovePerformanceType.MoveToTarget || _movePerformanceType == MovePerformanceType.MoveToPosition)
		{
			_isUseUpdate = true;
			_moveSpeed = performanceData.floatData[0];
		}
	}

	public override void OnStart()
	{
		_targetTransform = ownerChampion.targetChampion.transform;
		if (null != _targetTransform)
		{
			_targetPosition = _targetTransform.position;

			Vector3 dir = (_targetPosition - ownerChampion.transform.position).normalized;

			ownerChampion.blackboard.SetVectorValue(BlackboardKeyTable.EFFECT_DRIECTION, dir);
		}

		_isOnAction = false;
		isEndPerformance = false;
	}

	public override void OnUpdate()
	{
		if (false == _isUseUpdate)
			return;
		if (true == isEndPerformance || false == _isOnAction)
			return;
		if (null == ownerChampion || null == ownerChampion.targetChampion)
			return;

		switch (_movePerformanceType)
		{
			case MovePerformanceType.MoveToPosition:
				break;
			case MovePerformanceType.MoveToTarget:
				{
					_targetPosition = _targetTransform.position;
					Vector3 dir = (_targetPosition - ownerChampion.transform.position);
					float distance = dir.magnitude;

					if (distance <= ownerChampion.status.range)
					{
						isEndPerformance = true;

						ownerChampion.GetComponentInChildren<ChampionAnimation>().OnAnimationEnd();
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
		_isOnAction = true;

		if (false == _isUseUpdate)
		{
			if (null == ownerChampion || null == _targetTransform)
				return;

			switch (_movePerformanceType)
			{
				case MovePerformanceType.TeleportToPosition:
					{
						Vector3 dir = (_targetPosition - ownerChampion.transform.position).normalized;
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
	}

	public override void OnEnd()
	{
		
	}
}