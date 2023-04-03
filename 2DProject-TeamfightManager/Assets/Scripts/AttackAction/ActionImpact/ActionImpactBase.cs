using UnityEngine;

public abstract class ActionImpactBase
{
	public abstract void Impact(Champion target, in AttackImpactData impactData);
}