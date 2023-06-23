using UnityEngine;

/// <summary>
/// 공격 관련 효과를 적에게 적용한다..
/// </summary>
public class Impact_AttackDamage : ActionImpactBase
{
	public override void Impact(Champion target, in AttackImpactData impactData)
	{
		int damage = (int)(impactData.mainData.amount * ownerChampion.status.atkStat);

		target.TakeDamage(ownerChampion, damage);

		if (true == impactData.isShowEffect)
		{
			attackAction.ShowEffect(impactData.effectData, ownerChampion, target);
		}

		isEnded = true;
	}
}