using System;
using UnityEngine;

public class SummonStructure : SummonObject
{
	[SerializeField] protected float _actionTickTime = 0f;
	[SerializeField] protected float _lifeTime = 0f;
	[SerializeField] protected bool _isUseLifeTime = true;
	[SerializeField] protected bool _isAttachOwner = false;
	[SerializeField] protected bool _isSpawnTargetPosition = false;

	protected float _elapsedTickTime = 0f;
	protected float _elapsedTime = 0f;
	protected int _getLayerMask;

	protected Transform _ownerTransform;
	protected Vector3 _offsetPosition;

	protected void Update()
	{
		if(true == _isAttachOwner)
		{
			transform.position = _ownerTransform.position + _offsetPosition;
		}

		if (false == _isUseLifeTime)
			return;

		float deltaTime = Time.deltaTime;
		_elapsedTickTime += deltaTime;
		if (_elapsedTickTime >= _actionTickTime)
		{
			Action();
			_elapsedTickTime -= _actionTickTime;
		}

		_elapsedTime += deltaTime;
		if (_elapsedTime >= _lifeTime)
		{
			ReceiveReleaseEvent();
			summonObjectManager.ReleaseSummonObject(this);
		}
	}

	public virtual void SetAdditionalData(int layerMask, Champion target, Champion owner, Func<Vector3, Champion[], int> targetFindFunc)
	{
		Array.Clear(_targetArray, 0, _targetArray.Length);
		_elapsedTickTime = 0f;
		_elapsedTime = 0f;

		_targetFindFunc = targetFindFunc;

		_getLayerMask = layerMask;

		if (null != target && _isSpawnTargetPosition)
		{
			transform.position = target.transform.position;
		}
		if (null != owner && _isSpawnTargetPosition)
		{
			_offsetPosition = transform.position - owner.transform.position;
        }
	}

	protected virtual void Action()
	{
		int targetCount = _targetFindFunc.Invoke(transform.position, _targetArray);
		if (0 < targetCount)
		{
			ReceiveImpactExecuteEvent(targetCount);
		}
	}
}