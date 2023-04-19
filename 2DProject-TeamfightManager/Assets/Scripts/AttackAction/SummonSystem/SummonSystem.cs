using System.Collections.Generic;
using System;
using UnityEngine;

public class SummonSystem
{
	public ProjectileManager projectileManager { private get; set; }

	private AtkActionSummonData _summonData;
	private List<AttackImpactData> _impactDatas;
	private Champion[] _targetArray;
	private Func<int> _findTargetFunc;
	private Action<AttackImpactData, int, Champion[]> _impactFunc;

	private float _elapsedTime = 0f;

	public SummonSystem(AtkActionSummonData summonData, Func<int> findTargetFunc)
	{
		_summonData = summonData;
		_findTargetFunc = findTargetFunc;
	}

	public void OnStart(Champion[] targetCacheArray)
	{
		_targetArray = targetCacheArray;
		SummonObject();
	}

	public void Update()
	{
		if (false == _summonData.isSummonOnce)
		{
			_elapsedTime += Time.deltaTime;
			if(_elapsedTime >= _summonData.tickTime)
			{
				SummonObject();
				_elapsedTime -= _summonData.tickTime;
			}
		}
	}

	public void AddImpactData(AttackImpactData impactData)
	{
		_impactDatas.Add(impactData);
	}

	private void SummonObject()
	{

	}
}