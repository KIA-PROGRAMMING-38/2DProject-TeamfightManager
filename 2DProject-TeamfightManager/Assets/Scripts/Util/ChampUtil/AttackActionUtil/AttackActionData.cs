public enum ImpactRangeKind
{
	OnlyTarget,
	Range_Circle,
}

public enum ActionStartPointKind
{
	TargetPosition,
	MyPosition
}

public class AttackActionData
{
	public int uniqueKey;
	public bool isPassive;
	public float impactRange;
	public int impactRangeType;
	public int actionStartPointKind;
}