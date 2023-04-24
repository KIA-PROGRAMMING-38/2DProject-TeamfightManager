using UnityEngine;
/// <summary>
/// 현재 챔피언이 설정한 타겟을 적용 대상으로 처리한다..
/// </summary>
public class DecideTarget_OnlyTarget : AtkActionDecideTargetBase
{
	private BattleTeam battleTeam;

	public DecideTarget_OnlyTarget(AttackAction attackAction, AttackActionData actionData) : base(attackAction, actionData)
	{
	}

	public override void OnStart()
	{
		battleTeam = ownerChampion.pilotBattleComponent.myTeam;
	}

	public override int FindTarget(FindTargetData findTargetData, Champion[] getTargetArray)
	{
		// 인자로 받은 챔피언 캐시 배열이 빈 배열이라면..
		if (null == getTargetArray || 0 == getTargetArray.Length)
			return 0;

		Champion findTarget = battleTeam.FindTarget(ownerChampion, findTargetData);
		if(null != findTarget)
		{
			getTargetArray[0] = findTarget;
			return 1;
		}

		return 0;
	}

	public override int FindTarget(FindTargetData findTargetData, Champion[] getTargetArray, Vector3 startPoint)
	{
		return FindTarget(findTargetData, getTargetArray);
	}

	public override void OnEnd()
	{
		
	}
}