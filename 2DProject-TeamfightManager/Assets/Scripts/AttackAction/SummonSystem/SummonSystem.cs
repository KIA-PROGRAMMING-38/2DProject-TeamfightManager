using System.Collections.Generic;
using System;
using UnityEngine;

public class SummonSystem
{
	public ProjectileManager projectileManager { private get; set; }

	public Champion ownerChampion { private get; set; }

	private AttackAction _attackAction;
	private AtkActionSummonData _summonData;
	private List<AttackImpactData> _impactDatas = new List<AttackImpactData>();
	private Champion[] _targetArray;
	private Func<Vector3, Champion[], int> _findImpactTargetFunc;
	private Func<Champion> _findTargetFunc;
	private Action<AttackImpactData, int, Champion[]> _impactFunc;

	private float _elapsedTime = 0f;

	public TargetTeamKind atkImpactTeamKind { private get; set; }

	public SummonSystem(AttackAction attackAction, AtkActionSummonData summonData, Func<Vector3, Champion[], int> findImpactTargetFunc, Func<Champion> findTargetFunc)
	{
		_attackAction = attackAction;
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
			if (_elapsedTime >= _summonData.tickTime)
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
					Champion target = _findTargetFunc.Invoke();

					if (null != target && false == target.isDead)
					{
						Projectile projectile = projectileManager.GetProjectile(_summonData.summonObjectName);

						Vector3 moveDirection = _summonData.offsetPosition;
						int layer = (atkImpactTeamKind == TargetTeamKind.Team) ? ownerChampion.buffSummonLayer : ownerChampion.atkSummonLayer;
						if (ownerChampion.flipX)
							moveDirection.x *= -1f;

						projectile.gameObject.SetActive(true);

						projectile.transform.position = ownerChampion.transform.position + moveDirection;
						projectile.SetAdditionalData(layer, target, _findImpactTargetFunc);

						projectile.OnExecuteImpact -= OnProjectileExecuteImpact;
						projectile.OnExecuteImpact += OnProjectileExecuteImpact;
					}
				}

				break;
			case SummonObjectType.Structure:

				break;
		}
	}

	private void OnProjectileExecuteImpact(Projectile projectile, Champion[] targetArray, int targetCount)
	{
        projectile.OnExecuteImpact -= OnProjectileExecuteImpact;

        for (int targetIndex = 0; targetIndex < targetCount; ++targetIndex)
		{
			int impactCount = _impactDatas.Count;
            for ( int impactIndex = 0; impactIndex < impactCount; ++impactIndex)
			{
				Debug.Log("데미지를 준다.");

                AttackImpactData curImpactData = _impactDatas[impactIndex];

                // 찾은 타겟 개수만큼 효과 부여..
                _attackAction.ImpactTarget(curImpactData, targetCount, targetArray);
			}
		}
	}
}