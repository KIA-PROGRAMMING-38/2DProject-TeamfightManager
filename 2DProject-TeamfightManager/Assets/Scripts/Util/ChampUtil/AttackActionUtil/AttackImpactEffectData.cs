public enum AttackImpactEffectKind
{
	Buff,
	Debuff,
	Attack,
	End
}

[System.Serializable]
public class AttackImpactData
{
	public AttackImpactMainData mainData;		// 효과를 주는 것 관련 데이터..

	public bool isSeparateTargetFindLogic;      // 타겟 찾는 로직을 따로 구할 것인가..
	public FindTargetData findTargetData;		// 타겟 찾는 로직 관련 데이터..
}

[System.Serializable]
public class AttackImpactMainData
{
	public AttackImpactEffectKind kind;         // 공격 행동 시 어떤 효과를 줄 것인지 종류..
	public int detailKind;                      // kind의 값에 따라 다른 enum을 파싱해서 사용..
	public float amount;                        // 효과량(공격의 경우 데미지)
	public float duration;                      // 효과 지속 시간(디버프의 경우 얼마나 지속될 것인지)..
	public float tickTime;                      // 틱 시간(장판의 경우 도트뎀 들어가는 시간?, ticktime마다 도트뎀 들어가게끔)..
}