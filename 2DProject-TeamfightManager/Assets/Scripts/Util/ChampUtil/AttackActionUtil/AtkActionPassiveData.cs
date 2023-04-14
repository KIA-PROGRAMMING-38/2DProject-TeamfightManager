using System.Collections.Generic;

public enum PassiveImpactTimeKind
{
	OnMyTeamHit,
	OnKillTarget,
}

[System.Serializable]
public class AtkActionPassiveData
{
	public PassiveImpactTimeKind impactTimeKind;

	public List<AttackImpactData> alwaysImpactDatas;

	public bool isPlayUltAnim;
}