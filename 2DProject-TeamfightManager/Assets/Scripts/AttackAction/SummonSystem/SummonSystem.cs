using System.Collections.Generic;
using System;
using UnityEngine;

public class SummonSystem
{
	public ProjectileManager projectileManager { private get; set; }

	public Champion ownerChampion { private get; set; }

	private AtkActionSummonData _summonData;
	private List<AttackImpactData> _impactDatas = new List<AttackImpactData>();
	private Champion[] _targetArray;
	private Func<Vector3, int> _findImpactTargetFunc;
	private Func<Champion> _findTargetFunc;
	private Action<AttackImpactData, int, Champion[]> _impactFunc;

	private float _elapsedTime = 0f;

	public TargetTeamKind atkImpactTeamKind { private get; set; }

	public SummonSystem(AtkActionSummonData summonData, Func<Vector3, int> findImpactTargetFunc, Func<Champion> findTargetFunc)
	{
		_summonData = summonData;

		_findImpactTargetFunc = findImpactTargetFunc;
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
		switch (_summonData.summonObjectType)
		{
			case SummonObjectType.Champion:
				break;
			case SummonObjectType.Projectile:
				{
					Projectile projectile = projectileManager.GetProjectile(_summonData.summonObjectName);

					Vector3 moveDirection = _summonData.offsetPosition;
					int layer = (atkImpactTeamKind == TargetTeamKind.Team) ? ownerChampion.buffSummonLayer : ownerChampion.atkSummonLayer;
					if (ownerChampion.flipX)
						moveDirection.x *= -1f;
					projectile.transform.Translate(moveDirection);
					projectile.SetAdditionalData(layer, _findTargetFunc.Invoke(), _targetArray, _findImpactTargetFunc);
				}

				break;
			case SummonObjectType.Structure:
				break;
		}
	}
}