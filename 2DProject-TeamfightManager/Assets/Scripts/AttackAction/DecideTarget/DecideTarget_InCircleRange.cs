public class DecideTarget_InCircleRange : AtkActionDecideTargetBase
{
	public DecideTarget_InCircleRange(Champion champion, AttackActionData actionData) : base(champion, actionData)
	{

	}

	public override void OnStart()
	{

	}

	public override int FindTarget(Champion[] getTargetArray)
	{
		ActionStartPointKind startPointKind = (ActionStartPointKind)actionData.actionStartPointKind;
		switch(startPointKind)
		{
			case ActionStartPointKind.TargetPosition:

				break;

			case ActionStartPointKind.MyPosition:
				
				break;
		}

		return 0;
	}

	public override void OnEnd()
	{

	}
}