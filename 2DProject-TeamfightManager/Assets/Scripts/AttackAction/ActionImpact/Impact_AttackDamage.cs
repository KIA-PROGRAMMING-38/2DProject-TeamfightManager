using UnityEngine;

/// <summary>
/// 공격 관련 효과를 적에게 적용한다..
/// </summary>
public class Impact_AttackDamage : ActionImpactBase
{
	public override void Impact(Champion target, in AttackImpactData impactData)
	{
		AttackImpactType atkImpactType = (AttackImpactType)impactData.mainData.detailKind;

		switch (atkImpactType)
		{
			case AttackImpactType.DefaultAttack:
				target.TakeDamage(ownerChampion, (int)(impactData.mainData.amount * ownerChampion.status.atkStat));
				break;
			default:
				Debug.Assert(false, "Impact_AttackDamage's Impact() : Invalid Data");
				break;
		}

		isEnded = true;
	}
}