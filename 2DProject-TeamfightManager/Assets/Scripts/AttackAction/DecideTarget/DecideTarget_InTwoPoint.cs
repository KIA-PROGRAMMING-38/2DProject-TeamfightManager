using System.ComponentModel;
using UnityEngine;

/// <summary>
/// 효과를 받는 대상을 두 지점을 기준으로 네모를 만들어 그 안에 있는지 검사..
/// </summary>
public class DecideTarget_InTwoPoint : AtkActionDecideTargetBase
{
	private BattleTeam battleTeam;
	private Vector3 _beginPosition;

	private Vector3[] _boxPointsCache;

	public DecideTarget_InTwoPoint(AttackAction attackAction, AttackActionData actionData) : base(attackAction, actionData)
	{
		_boxPointsCache = new Vector3[4];
	}

	public override void OnStart()
	{
		if (null == battleTeam)
		{
			battleTeam = ownerChampion.pilotBattleComponent.myTeam;
		}

		_beginPosition = ownerChampion.transform.position;
	}

	public override int FindTarget(FindTargetData findTargetData, Champion[] getTargetArray)
	{
		Vector3 endPoint = ownerChampion.transform.position;

		Vector3 dir = endPoint - _beginPosition;
		float dirLength = dir.magnitude;
		dir /= dirLength;

		Vector3 rightDir = new Vector3(dir.y, dir.x, 0f) * findTargetData.impactRange;
		Vector3 leftDir = new Vector3(-dir.y, dir.x, 0f) * findTargetData.impactRange;

		_boxPointsCache[0] = _beginPosition + leftDir;
		_boxPointsCache[1] = _beginPosition + rightDir;
		_boxPointsCache[2] = endPoint + leftDir;
		_boxPointsCache[3] = endPoint + rightDir;

		bool TargetFindLogic(Vector3 enemyPosition)
		{
			return checkSquareInPoint(_boxPointsCache, findTargetData.impactRange, dirLength, enemyPosition);
		}

		// 범위 안에 들어오는 적을 찾는 로직..
		return battleTeam.ComputeEnemyTarget(TargetFindLogic, getTargetArray, findTargetData.targetTeamKind);
	}

	public override void OnEnd()
	{

	}

	private float getAreaOfTriangle(in Vector3 dot1, in Vector3 dot2, in Vector3 dot3)
	{
		Vector3 a = dot2 - dot1;
		Vector3 b = dot3 - dot1;
		Vector3 cross = Vector3.Cross(a, b);

		return cross.magnitude / 2.0f;
	}

	private bool checkSquareInPoint(Vector3[] boxPoints, float width, float height, in Vector3 point)
	{
		float area = width * height * 4f;

		float t1 = getAreaOfTriangle(point, boxPoints[0], boxPoints[1]);
		float t2 = getAreaOfTriangle(point, boxPoints[0], boxPoints[2]);
		float t3 = getAreaOfTriangle(point, boxPoints[1], boxPoints[3]);
		float t4 = getAreaOfTriangle(point, boxPoints[2], boxPoints[3]);

		return (t1 + t2 + t3 + t4) < area + 0.1f;
	}
}