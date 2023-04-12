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

		// 챔피언의 타겟을 찾지 못한 상태라면..
		if (null == ownerChampion.targetChampion)
			return 0;

		TargetTeamKind teamKind = (TargetTeamKind)findTargetData.targetTeamKind;

		switch (teamKind)
		{
			case TargetTeamKind.Enemy:
				getTargetArray[0] = attackAction.targetChampion;
				break;
			case TargetTeamKind.Team:
				getTargetArray[0] = ownerChampion;
				break;
		}

		return 1;
	}

	public override void OnEnd()
	{
		
	}
}