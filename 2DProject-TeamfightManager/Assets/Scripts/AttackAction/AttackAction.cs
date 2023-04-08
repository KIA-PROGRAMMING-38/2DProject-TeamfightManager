using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// 챔피언의 공격 행동 로직을 수행하는 클래스..
/// </summary>
public class AttackAction
{
	private static readonly ActionImpactBase[] s_actionImpactLogics;

	public Champion targetChampion { get; private set; }

	private AttackActionData _actionData;
	private List<AttackImpactData> _impactData;

	private ActionContinuousPerformance _contPerf;
	private AtkActionDecideTargetBase _decideTargetLogic;

	private Champion[] _findTargetsCache;

	private bool _isEndAction;
	private bool _isEndAnimation;

	public bool isEndAction { get => _isEndAnimation && _isEndAction; }

	private Champion _ownerChampion;
	public Champion ownerChampion
	{
		get => _ownerChampion;
		set
		{
#if UNITY_EDITOR
			Debug.Assert(null != _decideTargetLogic && null != value, "AttackAction's ownerChampion : invalid reference");

#endif
			_ownerChampion = value;

			_decideTargetLogic.ownerChampion = value;
			if (null != _contPerf)
				_contPerf.ownerChampion = value;

			int loopCount = (int)AttackImpactEffectKind.End;
			for( int i = 0; i < loopCount; ++i )
			{
				s_actionImpactLogics[i].ownerChampion = value;
			}
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
				_decideTargetLogic = new DecideTarget_OnlyTarget(this, _actionData);
				break;

			case ImpactRangeKind.Range_Circle:
				_decideTargetLogic = new DecideTarget_InCircleRange(this, _actionData);
				break;

			case ImpactRangeKind.Range_InTwoPointBox:
				_decideTargetLogic = new DecideTarget_InTwoPoint(this, _actionData);
				break;
		}

		if (true == performanceData.isUsePerf)
		{
			switch (performanceData.perfType)
			{
				case AttackPerformanceType.Move:
					_contPerf = new ContPerf_Move(this, performanceData);
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

		targetChampion = ownerChampion.targetChampion;
	}

	public void OnUpdate()
	{
		if(null == targetChampion)
		{
			_isEndAction = true;
			return;
		}

		if (null != _contPerf)
		{
			_contPerf.OnUpdate();
			_isEndAction = _contPerf.isEndPerformance;
		}
	}

	public void OnAction()
	{
		if (null == targetChampion)
		{
			_isEndAction = true;
			return;
		}

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

		targetChampion = null;
	}

	public void OnAnimationEndEvent()
	{
		_isEndAnimation = true;
	}
}