using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
	private float _lerpT = 0f;
	private float _moveDistance;
	private Vector2 _bezierPoint1 = new Vector2();
	private Vector2 _bezierPoint2 = new Vector2();
	private Vector2 _bezierPoint3 = new Vector2();

    public ContPerf_Move(AttackAction attackAction, AttackPerformanceData performanceData) : base(attackAction, performanceData)
	{
		_movePerformanceType = (MovePerformanceType)performanceData.detailType;

		if (_movePerformanceType == MovePerformanceType.MoveToTarget || _movePerformanceType == MovePerformanceType.MoveToPosition)
		{
			_isUseUpdate = true;
			_moveSpeed = performanceData.floatData[0];
		}
		else if (_movePerformanceType == MovePerformanceType.Jump)
		{
            _isUseUpdate = true;
            _moveSpeed = performanceData.floatData[0];
			_moveDistance = performanceData.floatData[1];
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

		if(MovePerformanceType.Jump == _movePerformanceType)
		{
			_lerpT = 0f;
			_bezierPoint1 = ownerChampion.transform.position;

            Vector2 dir = (ownerChampion.transform.position - _targetPosition).normalized;
			float distance = _moveDistance;

            RaycastHit2D hit = Physics2D.Raycast(_bezierPoint1, dir, distance, 1 << LayerTable.Number.STAGE_AREALIMITLINE);
            if (null != hit.collider)
            {
                distance = hit.distance;
            }

            _bezierPoint3 = _bezierPoint1 + dir * distance;

            // 두 점으로 정삼각형 만드는 공식(https://tibyte.kr/10 참고)을 좀 변형..
            _bezierPoint2.x = ((_bezierPoint1.x + _bezierPoint3.x) * 0.5f) + ((_bezierPoint3.y - _bezierPoint1.y) * Mathf.Sqrt(3f) * 0.2f);
			_bezierPoint2.y = ((_bezierPoint1.y + _bezierPoint3.y) * 0.5f) - ((_bezierPoint3.x - _bezierPoint1.x) * Mathf.Sqrt(3f) * 0.2f);

			dir = (_bezierPoint2 - _bezierPoint1);
			distance = dir.magnitude;
			dir /= distance;

            hit = Physics2D.Raycast(_bezierPoint1, dir, distance, 1 << LayerTable.Number.STAGE_AREALIMITLINE);
            if (null != hit.collider)
            {
				_bezierPoint2 = _bezierPoint1 + dir * hit.distance;
            }
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

            case MovePerformanceType.Jump:
                {
					_lerpT = Mathf.Min(1f, _lerpT + Time.deltaTime * _moveSpeed);

					Vector2 curPos = MathUtility.Bezier.QuadraticBezierCurve(_bezierPoint1, _bezierPoint2, _bezierPoint3, _lerpT);

                    if (_lerpT >= 1f)
                    {
						if (attackAction.isEndAnimation)
							isEndPerformance = true;
                    }
                    else
                    {
						ownerChampion.transform.position = curPos;
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

						RaycastHit2D hit = Physics2D.Raycast(ownerChampion.transform.position, dir, distance, 1 << LayerTable.Number.STAGE_AREALIMITLINE);
						if (null != hit.collider)
						{
							distance = hit.distance;
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