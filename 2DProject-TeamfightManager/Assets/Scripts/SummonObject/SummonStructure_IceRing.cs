using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonStructure_IceRing : SummonStructure
{
	private AttackImpactMainData lastFreezeImpactData;

	new private void Awake()
	{
		base.Awake();

		OnRelease -= ActLastFreezeImpact;
		OnRelease += ActLastFreezeImpact;

		lastFreezeImpactData = new AttackImpactMainData
		{
			kind = AttackImpactEffectKind.Debuff,
			detailKind = (int)DebuffImpactType.Freeze,
			amount = 0,
			duration = 1.5f,
			tickTime = 0
		};
	}

	private void ActLastFreezeImpact(SummonObject summonObject)
	{
		int targetCount = _targetFindFunc.Invoke(transform.position, _targetArray);
		if (0 < targetCount)
		{
			for( int i = 0; i < targetCount; ++i)
			{
				Champion champion = _targetArray[i].GetComponent<Champion>();
				if (null == champion || champion.isDead)
					continue;

				champion.AddDebuff(lastFreezeImpactData, null);
			}
		}
	}
}
