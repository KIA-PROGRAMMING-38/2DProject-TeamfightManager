using MH_AIFramework;

public class DN_CheckIsCanAttack : DecoratorNode
{
	private float _attackRange = 0.0f;

	public override void OnCreate()
	{
		base.OnCreate();
	}

	protected override void OnStart()
	{
		_attackRange = blackboard.GetFloatValue( "attackRange" );
	}

	protected override void OnStop()
	{
		
	}

	protected override State OnUpdate()
	{
		if (null == blackboard.GetObjectValue("target"))
			return State.Failure;

		// 타겟과의 거리가 나의 공격 범위보다 작다면..
		float dist = blackboard.GetFloatValue( "targetDistance" );
		if ( _attackRange < dist )
			return State.Failure;

		// 만약 공격 쿨타임이라면..
		if ( false == blackboard.GetBoolValue( "isAttack" ) )
			return State.Failure;

		return State.Success;
	}
}
