using System;
using UnityEngine;

public class SummonStructure : SummonObject
{
	[SerializeField] protected float _actionTickTime = 0f;
	[SerializeField] protected float _lifeTime = 0f;
	[SerializeField] protected float _elapsedTickTime = 0f;
	[SerializeField] protected float _elapsedTime = 0f;
	[SerializeField] protected bool _isUseLifeTime = true;
	[SerializeField] protected bool _isAttachTarget = false;

	private void Update()
	{
		if (false == _isUseLifeTime)
			return;

		float deltaTime = Time.time;
		_elapsedTickTime += deltaTime;
		if (_elapsedTickTime >= _actionTickTime)
		{
			Action();
			_elapsedTickTime -= _actionTickTime;
		}

		_elapsedTime += deltaTime;
		if (_elapsedTime < _lifeTime)
		{
			ReceiveReleaseEvent();
			summonObjectManager.ReleaseSummonObject(this);
		}
	}

	public void SetAdditionalData(int layerMask, Champion target, Func<Vector3, Champion[], int> targetFindFunc)
	{
		Array.Clear(_targetArray, 0, _targetArray.Length);
		_elapsedTickTime = 0f;
		_elapsedTime = 0f;

		gameObject.layer = layerMask;
		_targetFindFunc = targetFindFunc;
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