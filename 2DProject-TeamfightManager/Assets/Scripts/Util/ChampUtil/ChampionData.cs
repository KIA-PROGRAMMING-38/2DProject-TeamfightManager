[System.Serializable]
public class ChampionData
{
	public string name;                     // 챔피언 이름..
	public string nameKR;					// 챔피언 한글 이름..
	public ChampionClassType type;			// 챔피언 클래스 종류..

	public int atkActionUniqueKey;			// 공격 행동 키 값..
	public int skillActionUniqueKey;		// 공격 행동 키 값..
	public int ultimateActionUniqueKey;		// 공격 행동 키 값..

	public string champDescription;			// 챔피언 설명..

	public FindTargetData findTargetData;	// 챔피언 기본 타겟 찾는 로직에 필요한 정보들..
}