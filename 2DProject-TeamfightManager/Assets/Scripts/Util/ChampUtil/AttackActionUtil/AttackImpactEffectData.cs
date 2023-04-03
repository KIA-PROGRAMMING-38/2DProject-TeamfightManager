public enum AttackImpactEffectKind
{
	Buff,
	Debuff,
	Attack,
	End
}

public enum AttackImpactType
{
	DefaultAttack
}

public enum TargetDecideKind
{
	OnlyTarget
}

public class AttackImpactData
{
	public int kind;				// AttackImpactEffectKind의 값을 int로 변환해서 담는다(꺼내온 다음에는 AttackImpactEffectKind로 변환)..
	public int detailKind;          // kind의 값에 따라 다른 enum을 파싱해서 사용..
	public int amount;				// 효과량(공격의 경우 데미지)
	public float duration;			// 효과 지속 시간(디버프의 경우 얼마나 지속될 것인지)..
	public float tickTime;			// 틱 시간(장판의 경우 도트뎀 들어가는 시간?, ticktime마다 도트뎀 들어가게끔)..
	public int targetDecideKind;	// 타겟 결정 방식(타겟 하나만 공격할 지 주변 범위의 적 모두를 공격할지 등등)..
}