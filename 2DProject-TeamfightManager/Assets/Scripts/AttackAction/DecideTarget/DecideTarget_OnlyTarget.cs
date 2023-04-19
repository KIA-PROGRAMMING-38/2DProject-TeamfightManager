﻿using UnityEngine;
/// <summary>
/// 현재 챔피언이 설정한 타겟을 적용 대상으로 처리한다..
/// </summary>
public class DecideTarget_OnlyTarget : AtkActionDecideTargetBase
{
	public DecideTarget_OnlyTarget(AttackAction attackAction, AttackActionData actionData) : base(attackAction, actionData)
	{
	}

	public override void OnStart()
	{

	}

	public override int FindTarget(FindTargetData findTargetData, Champion[] getTargetArray)
	{
		// 인자로 받은 챔피언 캐시 배열이 빈 배열이라면..
		if (null == getTargetArray || 0 == getTargetArray.Length)
			return 0;

		TargetTeamKind teamKind = (TargetTeamKind)findTargetData.targetTeamKind;

		switch (teamKind)
		{
			case TargetTeamKind.Enemy:
				if (null == ownerChampion.targetChampion)
					return 0;

				getTargetArray[0] = attackAction.targetChampion;

				break;
			case TargetTeamKind.Team:
				if (null == ownerChampion)
					return 0;

				getTargetArray[0] = ownerChampion;

				break;
		}

		return 1;
	}

	public override int FindTarget(FindTargetData findTargetData, Champion[] getTargetArray, Vector3 startPoint)
	{
		return FindTarget(findTargetData, getTargetArray);
	}

	public override void OnEnd()
	{
		
	}
}