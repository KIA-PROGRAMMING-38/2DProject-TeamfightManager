using System.Numerics;

public enum AttackMoveType
{
	MoveToTarget,
	MoveToPosition
}

public class AttackMoveData
{
	public AttackMoveType moveType;	// 움직임 타입..
	public Vector3 moveDirection;	// 움직일 방향 + 거리..
	public float moveSpeed;			// 스피드가 0인 경우 순간이동..
}