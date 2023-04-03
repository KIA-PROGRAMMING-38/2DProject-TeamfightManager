using UnityEngine;

public class Impact_AttackDamage : ActionImpactBase
{
	public override void Impact(Champion target, in AttackImpactData impactData)
	{
		AttackImpactType atkImpactType = (AttackImpactType)impactData.detailKind;

		switch (atkImpactType)
		{
			case AttackImpactType.DefaultAttack:
				target.Hit(impactData.amount);
				break;
			default:
				Debug.Assert(false, "Impact_AttackDamage's Impact() : Invalid Data");
				break;
		}
	}
}