using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Timeline.Actions;

/// <summary>
/// 챔피언의 공격 행동 로직을 수행하는 클래스..
/// </summary>
public class AttackAction
{
	public Champion targetChampion { get; private set; }

	private AttackActionData _actionData;
	private List<AttackImpactData> _impactData;
	private ActionImpactBase[] _actionImpactLogics;

	private ActionContinuousPerformance _contPerf;

	private AtkActionDecideTargetBase[] _decideTargetLogicContainer;
	private AtkActionDecideTargetBase _baseDecideTargetLogic;
	private int _decideTargetLogicContainerCount = 0;

	public Champion[] _findTargetsCache { get; private set; }

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
			Debug.Assert(null != _decideTargetLogicContainer && null != value, "AttackAction's ownerChampion : invalid reference");

#endif
			_ownerChampion = value;

			for (int i = 0; i < _decideTargetLogicContainerCount; ++i)
				_decideTargetLogicContainer[i].ownerChampion = value;

			if (null != _contPerf)
				_contPerf.ownerChampion = value;

			int loopCount = (int)AttackImpactEffectKind.End;
			for (int i = 0; i < loopCount; ++i)
			{
				_actionImpactLogics[i].ownerChampion = value;
			}
		}
	}

	public AttackAction(AttackActionData attackActionData, AttackPerformanceData performanceData)
	{
		_actionData = attackActionData;
		_impactData = new List<AttackImpactData>();

		_findTargetsCache = new Champion[10];

		TargetDecideKind impactRangeKind = (TargetDecideKind)_actionData.findTargetData.targetDecideKind;

		_decideTargetLogicContainerCount = (int)TargetDecideKind.End;
		_decideTargetLogicContainer = new AtkActionDecideTargetBase[_decideTargetLogicContainerCount];

		_decideTargetLogicContainer[(int)TargetDecideKind.OnlyTarget] = new DecideTarget_OnlyTarget(this, attackActionData);
		_decideTargetLogicContainer[(int)TargetDecideKind.Range_Circle] = new DecideTarget_InCircleRange(this, attackActionData);
		_decideTargetLogicContainer[(int)TargetDecideKind.Range_InTwoPointBox] = new DecideTarget_InTwoPoint(this, attackActionData);

		_baseDecideTargetLogic = _decideTargetLogicContainer[(int)_actionData.findTargetData.targetDecideKind];

		if (true == performanceData.isUsePerf)
		{
			switch (performanceData.perfType)
			{
				case AttackPerformanceType.Move:
					_contPerf = new ContPerf_Move(this, performanceData);
					break;
			}
		}

		_actionImpactLogics = new ActionImpactBase[(int)AttackImpactEffectKind.End];
		_actionImpactLogics[(int)AttackImpactEffectKind.Buff] = new Impact_Debuff();
		_actionImpactLogics[(int)AttackImpactEffectKind.Debuff] = new Impact_Debuff();
		_actionImpactLogics[(int)AttackImpactEffectKind.Attack] = new Impact_AttackDamage();
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

		for (int i = 0; i < _decideTargetLogicContainerCount; ++i)
			_decideTargetLogicContainer[i].OnStart();
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

		int findTargetCount = _baseDecideTargetLogic.FindTarget(_actionData.findTargetData, _findTargetsCache);
		for (int i = 0; i < findTargetCount; ++i)
		{
			int jLoopCount = _impactData.Count;
			for (int j = 0; j < jLoopCount; ++j)
			{
				_actionImpactLogics[(int)_impactData[j].mainData.kind].Impact(_findTargetsCache[i], _impactData[j]);
			}
		}
	}

	public void OnEnd()
	{
		for (int i = 0; i < _decideTargetLogicContainerCount; ++i)
			_decideTargetLogicContainer[i].OnEnd();

		_contPerf?.OnEnd();

		targetChampion = null;
	}

	public void OnAnimationEndEvent()
	{
		_isEndAnimation = true;
	}
}