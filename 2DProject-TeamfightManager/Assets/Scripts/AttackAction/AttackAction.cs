using System.Collections.Generic;

public class AttackAction
{
	private static readonly ActionImpactBase[] s_actionImpactLogics;

	private AttackActionData _actionData;
	private List<AttackImpactData> _impactData;

	private AtkActionDecideTargetBase _decideTargetLogic;

	private Champion[] _findTargetsCache;

	static AttackAction()
	{
		s_actionImpactLogics = new ActionImpactBase[(int)AttackImpactEffectKind.End];
		s_actionImpactLogics[(int)AttackImpactEffectKind.Buff] = null;
		s_actionImpactLogics[(int)AttackImpactEffectKind.Debuff] = null;
		s_actionImpactLogics[(int)AttackImpactEffectKind.Attack] = new Impact_AttackDamage();
	}

	public AttackAction(Champion ownerChampion, AttackActionData attackActionData)
	{
		_actionData = attackActionData;
		_impactData = new List<AttackImpactData>();

		_findTargetsCache = new Champion[10];

		ImpactRangeKind impactRangeKind = (ImpactRangeKind)_actionData.impactRangeType;

		switch (impactRangeKind)
		{
			case ImpactRangeKind.OnlyTarget:
				_decideTargetLogic = new DecideTarget_OnlyTarget(ownerChampion, _actionData);
				break;

			case ImpactRangeKind.Range_Circle:
				_decideTargetLogic = new DecideTarget_InCircleRange(ownerChampion, _actionData);
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
