using UnityEngine;

public class ChampionData
{
	public string name;						// 챔피언 이름..
	public ChampionClassType type;			// 챔피언 클래스 종류..

	public string atkEffectName;			// 공격 이펙트 이름..
	public string skillEffectName;			// 스킬 이펙트 이름..
	public string ultimateEffectName;		// 궁극기 이펙트 이름..

	public int atkActionUniqueKey;			// 공격 행동 키 값..
	public int skillActionUniqueKey;		// 공격 행동 키 값..
	public int ultimateActionUniqueKey;		// 공격 행동 키 값..

	public string champDescription;			// 챔피언 설명..
}

public class ChampionResourceData
{
	public string champIconImagePath;		// 챔피언 아이콘 이미지 경로..
	public string skillIconImagePath;		// 스킬 아이콘 이미지 경로..
	public string ultimateIconImagePath;	// 궁극기 아이콘 이미지 경로..
}