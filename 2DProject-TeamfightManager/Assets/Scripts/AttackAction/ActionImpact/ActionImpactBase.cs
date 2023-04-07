using UnityEngine;

/// <summary>
/// 효과를 받을 대상에게 효과를 적용하는 로직의 최상위 클래스..
/// </summary>
public abstract class ActionImpactBase
{
	public abstract void Impact(Champion target, in AttackImpactData impactData);
}