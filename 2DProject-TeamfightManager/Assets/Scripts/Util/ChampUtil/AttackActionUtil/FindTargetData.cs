[System.Serializable]
public class FindTargetData
{
	public TargetDecideKind targetDecideKind;               // 타겟 결정 방식(타겟 하나만 공격할 지 주변 범위의 적 모두를 공격할지 등등)..
	public TargetTeamKind targetTeamKind;					// 타겟 팀 종류(타겟이 적인지 아군인지)..
	public float impactRange;								// 효과 범위(공격의 경우 공격 범위)..
	public ActionStartPointKind actionStartPointKind;       // 누구의 좌표를 기준점으로 잡을건지..
}