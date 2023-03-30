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

		// Ÿ�ٰ��� �Ÿ��� ���� ���� �������� �۴ٸ�..
		float dist = blackboard.GetFloatValue( "targetDistance" );
		if ( _attackRange < dist )
			return State.Failure;

		// ���� ���� ��Ÿ���̶��..
		if ( false == blackboard.GetBoolValue( "isAttack" ) )
			return State.Failure;

		return State.Success;
	}
}
