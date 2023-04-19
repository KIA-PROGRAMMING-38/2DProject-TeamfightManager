public enum AttackImpactType
{
	DefaultAttack
}

public enum BuffImpactType
{
	Hill,
	DefenceUp_Add,
	DefenceUp_Percent,
	AtkStatUp_Add,
	AtkStatUp_Percent,
	AtkSpeedUp_Percent,
	RangeUp_Percent,
	MoveSpeedUp_Percent,

	End,

	Barrier,				// 배리어..
	Barrier_MoveSpeed,      // 배리어가 유지되는 동안 증가되는 스피드..

	Barrier_End,
}

public enum DebuffImpactType
{
	HillAmount_Percent,
	DefenceDown_Add,
	DefenceDown_Percent,
	AtkStatDown_Add,
	AtkStatDown_Percent,
	AtkSpeedDown_Percent,
	MoveSpeedDown_Percent,

	Provoke,					// 도발..

	End
}

public enum TargetDecideKind
{
    OnlyTarget,
    Range_Circle,
    Range_InTwoPointBox,
	Random,
    End
}

public enum ActionStartPointKind
{
    TargetPosition,
    MyPosition
}

public enum TargetTeamKind
{
    Enemy,
    Team,
	Both
}