using System.Collections.Generic;
using System.Diagnostics;

public class AttackAction
{
	private static readonly ActionImpactBase[] s_actionImpactLogics;

	private AttackActionData _actionData;
	private List<AttackImpactData> _impactData;

	private AtkActionDecideTargetBase _decideTargetLogic;

	private Champion[] _findTargetsCache;

	public Champion ownerChampion
	{
		set
		{
#if UNITY_EDITOR
			Debug.Assert(null != _decideTargetLogic && null != value, "AttackAction's ownerChampion : invalid reference");

#endif
			_decideTargetLogic.ownerChampion = value;
		}
	}

	static AttackAction()
	{
		s_actionImpactLogics = new ActionImpactBase[(int)AttackImpactEffectKind.End];
		s_actionImpactLogics[(int)AttackImpactEffectKind.Buff] = new Impact_Debuf();
		s_actionImpactLogics[(int)AttackImpactEffectKind.Debuff] = new Impact_Debuf();
		s_actionImpactLogics[(int)AttackImpactEffectKind.Attack] = new Impact_AttackDamage();
	}

	public AttackAction(AttackActionData attackActionData)
	{
		_actionData = attackActionData;
		_impactData = new List<AttackImpactData>();

		_findTargetsCache = new Champion[10];

		ImpactRangeKind impactRangeKind = (ImpactRangeKind)_actionData.impactRangeType;

		switch (impactRangeKind)
		{
			case ImpactRangeKind.OnlyTarget:
				_decideTargetLogic = new DecideTarget_OnlyTarget(_actionData);
				break;

			case ImpactRangeKind.Range_Circle:
				_decideTargetLogic = new DecideTarget_InCircleRange(_actionData);
				break;
		}
	}

	public void AddImpactData(AttackImpactData impactData)
	{
		_impactData.Add(impactData);
	}

	public void RemoveImpactData(AttackImpactData impactData)
	{
		_impactData.Remove(impactData);
	}

	public void OnStart()
	{
		_decideTargetLogic.OnStart();
	}

	public void OnAction()
	{
		int findTargetCount = _decideTargetLogic.FindTarget(_findTargetsCache);
		for (int i = 0; i < findTargetCount; ++i)
		{
			int jLoopCount = _impactData.Count;
			for( int j = 0; j < jLoopCount; ++j)
			{
				s_actionImpactLogics[_impactData[j].kind].Impact(_findTargetsCache[i], _impactData[j]);
			}
		}
	}

	public void OnEnd()
	{
		_decideTargetLogic.OnEnd();
	}
}
