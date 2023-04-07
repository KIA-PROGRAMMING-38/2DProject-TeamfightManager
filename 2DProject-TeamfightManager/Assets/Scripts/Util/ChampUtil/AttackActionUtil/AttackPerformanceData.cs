using UnityEngine;

public enum MovePerformanceType
{
	MoveToPosition,
	TeleportToPosition,
	MoveToTarget,
	TeleportToTarget,
}

public enum AttackPerformanceType
{
	Move
}

public class AttackPerformanceData
{
	public bool isUsePerf;					// 퍼포먼스 사용하는지 여부..
	public AttackPerformanceType perfType;  // 퍼포먼스 타입..
	public int detailType;					// 퍼포먼스 디테일 타입..
	public Vector3[] vectorData;			// 퍼포먼스에 따라 사용될 vector3 데이터..
	public float[] floatData;               // 퍼포먼스에 따라 사용될 float 데이터..
}