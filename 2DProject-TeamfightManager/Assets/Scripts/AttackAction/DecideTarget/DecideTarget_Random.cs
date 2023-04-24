using UnityEngine;
/// <summary>
/// 현재 챔피언이 설정한 타겟을 적용 대상으로 처리한다..
/// </summary>
public class DecideTarget_Random : AtkActionDecideTargetBase
{
    private BattleTeam battleTeam;

    public DecideTarget_Random(AttackAction attackAction, AttackActionData actionData) : base(attackAction, actionData)
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
		Debug.Log("랜덤 적 찾는다2");

		Champion findTarget = battleTeam.ComputeRandomEnemyInRange(startPoint, findTargetData.impactRange, findTargetData.targetTeamKind);
        if (null != findTargetData)
        {
            getTargetArray[0] = findTarget;

            return 1;
        }

        return 0;
    }

    public override void OnEnd()
    {

    }
}