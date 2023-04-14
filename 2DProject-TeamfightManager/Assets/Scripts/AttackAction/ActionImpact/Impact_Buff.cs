public class Impact_Buff : ActionImpactBase
{
	public override void Impact(Champion target, in AttackImpactData impactData)
	{
		target.AddBuff(impactData.mainData, ownerChampion);

		if (true == impactData.isShowEffect)
		{
			attackAction.ShowEffect(impactData.effectData, ownerChampion, target);
		}
	}
}