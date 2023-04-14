/// <summary>
/// 디버프 관련 효과를 적에게 적용한다..
/// </summary>
public class Impact_Debuff : ActionImpactBase
{
	public override void Impact(Champion target, in AttackImpactData impactData)
	{
		target.AddDebuff(impactData.mainData, ownerChampion);

		if (true == impactData.isShowEffect)
		{
			attackAction.ShowEffect(impactData.effectData, ownerChampion, target);
		}
	}
}