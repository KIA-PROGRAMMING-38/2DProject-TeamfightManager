public enum ImpactRangeKind
{
	OnlyTarget,
	Range_Circle,
	Range_InTwoPointBox
}

public enum ActionStartPointKind
{
	TargetPosition,
	MyPosition
}

public class AttackActionData
{
	public int uniqueKey;				// 공격행동 고유의 키 값(이 것을 사용해 데이터 테이블
	public bool isPassive;				// 패시브인지 액티브인지..
	public float impactRange;			// 효과 범위(공격의 경우 공격 범위)..
	public int impactRangeType;			// 효과 범위 타입(타겟만 공격하는지 혹은 범위 공격인지 등등)..
	public int actionStartPointKind;	// 누구의 좌표를 기준점으로 잡을건지..
}