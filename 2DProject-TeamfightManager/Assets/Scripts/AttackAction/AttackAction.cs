using System.Collections.Generic;
using System.Diagnostics;

public class AttackAction
{
	private static readonly ActionImpactBase[] s_actionImpactLogics;

	private AttackActionData _actionData;
	private List<AttackImpactData> _impactData;
	private ActionContinuousPerformance _contPerf;

	private AtkActionDecideTargetBase _decideTargetLogic;

	private Champion[] _findTargetsCache;

	private bool _isEndAction;
	private bool _isEndAnimation;

	public bool isEndAction { get => _isEndAnimation && _isEndAction; }

	public Champion ownerChampion
	{
		set
		{
#if UNITY_EDITOR
			Debug.Assert(null != _decideTargetLogic && null != value, "AttackAction's ownerChampion : invalid reference");

#endif
			_decideTargetLogic.ownerChampion = value;
			if (null != _contPerf)
				_contPerf.ownerChampion = value;
		}
	}

	static AttackAction()
	{
		s_actionImpactLogics = new ActionImpactBase[(int)AttackImpactEffectKind.End];
		s_actionImpactLogics[(int)AttackImpactEffectKind.Buff] = new Impact_Debuf();
		s_actionImpactLogics[(int)AttackImpactEffectKind.Debuff] = new Impact_Debuf();
		s_actionImpactLogics[(int)AttackImpactEffectKind.Attack] = new Impact_AttackDamage();
	}

	public AttackAction(AttackActionData attackActionData, AttackPerformanceData performanceData)
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

			case ImpactRangeKind.Range_InTwoPointBox:
				_decideTargetLogic = new DecideTarget_InTwoPoint(_actionData);
				break;
		}

		if (true == performanceData.isUsePerf)
		{
			switch (performanceData.perfType)
			{
				case AttackPerformanceType.Move:
					_contPerf = new ContPerf_Move(performanceData);
					break;
			}
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
		_isEndAction = false;
		_isEndAnimation = false;

		_decideTargetLogic.OnStart();
		_contPerf?.OnStart();
	}

	public void OnUpdate()
	{
		if (null != _contPerf)
		{
			_contPerf.OnUpdate();
		}
	}

	public void OnAction()
	{
		if (null == _contPerf)
		{
			_isEndAction = true;
		}
		else
		{
			_contPerf.OnAction();
			_isEndAction = _contPerf.isEndPerformance;
		}

		int findTargetCount = _decideTargetLogic.FindTarget(_findTargetsCache);
		for (int i = 0; i < findTargetCount; ++i)
		{
			int jLoopCount = _impactData.Count;
			for (int j = 0; j < jLoopCount; ++j)
			{
				s_actionImpactLogics[_impactData[j].kind].Impact(_findTargetsCache[i], _impactData[j]);
			}
		}
	}

	public void OnEnd()
	{
		_decideTargetLogic.OnEnd();
		_contPerf?.OnEnd();
	}

	public void OnAnimationEndEvent()
	{
		_isEndAnimation = true;
	}
}