using UnityEngine;

public class DecideTarget_InCircleRange : AtkActionDecideTargetBase
{
	private BattleTeam battleTeam;

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
		if (null == ownerChampion)
			return 0;

		ActionStartPointKind startPointKind = (ActionStartPointKind)actionData.actionStartPointKind;
		Vector3 startPoint = Vector3.zero;

		switch (startPointKind)
		{
			case ActionStartPointKind.TargetPosition:
				if (null == ownerChampion.targetChampion)
					return 0;

				startPoint = ownerChampion.targetChampion.transform.position;

				break;
			case ActionStartPointKind.MyPosition:
				startPoint = ownerChampion.transform.position;

				break;
		}

		// 범위 안에 들어오는 적을 찾는 로직..
		bool TargetFindLogic(Vector3 enemyPosition)
		{
			float dist = (startPoint - enemyPosition).magnitude;
			return (dist <= actionData.impactRange);
		}

		return battleTeam.ComputeEnemyTarget(TargetFindLogic, getTargetArray);
	}

	public override void OnEnd()
	{

	}
}