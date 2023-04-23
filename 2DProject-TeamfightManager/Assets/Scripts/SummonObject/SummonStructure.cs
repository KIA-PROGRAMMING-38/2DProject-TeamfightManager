using System;
using UnityEngine;

public class SummonStructure : SummonObject
{
	[SerializeField] protected float _actionTickTime = 0f;
	[SerializeField] protected float _lifeTime = 0f;
	[SerializeField] protected bool _isUseLifeTime = true;
	[SerializeField] protected bool _isAttachTarget = false;

	protected float _elapsedTickTime = 0f;
	protected float _elapsedTime = 0f;
	protected int _getLayerMask;

	private void Update()
	{
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

	public void SetAdditionalData(int layerMask, Func<Vector3, Champion[], int> targetFindFunc)
	{
		Array.Clear(_targetArray, 0, _targetArray.Length);
		_elapsedTickTime = 0f;
		_elapsedTime = 0f;

		_targetFindFunc = targetFindFunc;

		_getLayerMask = layerMask;
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