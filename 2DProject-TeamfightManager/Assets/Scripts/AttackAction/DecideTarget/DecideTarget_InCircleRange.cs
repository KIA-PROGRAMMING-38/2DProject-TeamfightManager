using UnityEngine;

public class DecideTarget_InCircleRange : AtkActionDecideTargetBase
{
	BattleTeam battleTeam;

	public DecideTarget_InCircleRange(AttackActionData actionData) : base(actionData)
	{
		
	}

	public override void OnStart()
	{
		if (null == battleTeam)
		{
			battleTeam = ownerChampion.pilotBattleComponent.myTeam;
		}
	}

	public override int FindTarget(Champion[] getTargetArray)
	{
		ActionStartPointKind startPointKind = (ActionStartPointKind)actionData.actionStartPointKind;
		Vector3 startPoint = Vector3.zero;

		switch (startPointKind)
		{
			case ActionStartPointKind.TargetPosition:
				startPoint = ownerChampion.targetChampion.transform.position;

				break;
			case ActionStartPointKind.MyPosition:
				startPoint = ownerChampion.transform.position;

				break;
		}

		return battleTeam.ComputeInCircleRangeEnemyTarget(startPoint, actionData.impactRange, getTargetArray); ;
	}

	public override void OnEnd()
	{

	}
}