[System.Serializable]
public class AttackActionData
{
	public int uniqueKey;						// 공격행동 고유의 키 값(이 것을 사용해 데이터 테이블
	public FindTargetData findTargetData;       // 타겟 찾는 것 관련 데이터..
	public string description;                  // 스킬 설명..
	public bool isInvincible;					// 행동 중 무적 상태인가..
	public AtkRangeType rangeType;
	public float atkRange;

	public bool isSummon;
	public AtkActionSummonData summonData;

	public bool isPassive;                      // 패시브인지 액티브인지..
	public AtkActionPassiveData passiveData;    // 패시브 공격행동인 경우 필요한 데이터..
}

public enum AtkRangeType
{
	FollowDefaultRange,
	AllMapRange,
	CustomRange,
}