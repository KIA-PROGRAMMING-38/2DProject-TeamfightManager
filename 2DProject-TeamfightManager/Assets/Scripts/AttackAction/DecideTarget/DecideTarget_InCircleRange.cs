using UnityEngine;

/// <summary>
/// 효과를 받는 대상을 원 범위에 있는지 검사한다..
/// </summary>
public class DecideTarget_InCircleRange : AtkActionDecideTargetBase
{
	private BattleTeam battleTeam;

	public DecideTarget_InCircleRange(AttackAction attackAction, AttackActionData actionData) : base(attackAction, actionData)
	{
		
	}

	public override void OnStart()
	{
		battleTeam = ownerChampion.pilotBattleComponent.myTeam;
	}

	public override int FindTarget(FindTargetData findTargetData, Champion[] getTargetArray)
	{
		if (null == ownerChampion)
			return 0;

		ActionStartPointKind startPointKind = (ActionStartPointKind)findTargetData.actionStartPointKind;
		Vector3 startPoint = Vector3.zero;

		switch (startPointKind)
		{
			case ActionStartPointKind.TargetPosition:
				if (null == attackAction.targetChampion)
					return 0;

				startPoint = attackAction.targetChampion.transform.position;

				break;
			case ActionStartPointKind.MyPosition:
				startPoint = ownerChampion.transform.position;

				break;
		}

		return FindTarget(findTargetData, getTargetArray, startPoint);
	}

	public override int FindTarget(FindTargetData findTargetData, Champion[] getTargetArray, Vector3 startPoint)
	{
		if (null == ownerChampion)
			return 0;

		// 범위 안에 들어오는 적을 찾는 로직..
		bool TargetFindLogic(Vector3 enemyPosition)
		{
			float dist = (startPoint - enemyPosition).magnitude;
			return (dist <= findTargetData.impactRange);
		}

		return battleTeam.ComputeEnemyTarget(TargetFindLogic, getTargetArray, findTargetData.targetTeamKind);
	}

	public override void OnEnd()
	{

	}
}