﻿[System.Serializable]
public class AttackActionData
{
	public int uniqueKey;					// 공격행동 고유의 키 값(이 것을 사용해 데이터 테이블
	public bool isPassive;					// 패시브인지 액티브인지..
	public FindTargetData findTargetData;	// 타겟 찾는 것 관련 데이터..
}