using UnityEngine;

public class DecideTarget_OnlyTarget : AtkActionDecideTargetBase
{
	public DecideTarget_OnlyTarget(AttackActionData actionData) : base(actionData)
	{

	}

	public override void OnStart()
	{

	}

	public override int FindTarget(Champion[] getChampionArray)
	{
		// 인자로 받은 챔피언 캐시 배열이 빈 배열이라면..
		if (null == getChampionArray || 0 == getChampionArray.Length)
			return 0;

		// 챔피언의 타겟을 찾지 못한 상태라면..
		if (null == ownerChampion.targetChampion)
			return 0;

		// 챔피언의 타겟 넣어주고 리턴..
		getChampionArray[0] = ownerChampion.targetChampion;

		return 1;
	}

	public override void OnEnd()
	{
		
	}
}