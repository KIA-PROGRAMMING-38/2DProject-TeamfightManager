using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 챔피언의 공격 행동 로직을 수행하는 클래스..
/// </summary>
public class AttackAction
{
	public EffectManager effectManager { private get; set; }

	public Champion targetChampion { get; private set; }

	private AttackActionData _actionData;
	private AttackActionEffectData _effectData;
	private List<AttackImpactData> _impactData;
	private ActionImpactBase[] _actionImpactLogics;

	private ActionContinuousPerformance _contPerf;

	private AtkActionDecideTargetBase[] _decideTargetLogicContainer;
	private int _baseDecideTargetLogicIndex;
	private int _decideTargetLogicContainerCount = 0;

	public Champion[] baseFindTargetsCache { get; private set; }
	private Champion[][] _allLogicsFindTargetsCache;

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
			{
				if (null != _decideTargetLogicContainer[i])
					_decideTargetLogicContainer[i].ownerChampion = value;
			}

			if (null != _contPerf)
				_contPerf.ownerChampion = value;

			int loopCount = (int)AttackImpactEffectKind.End;
			for (int i = 0; i < loopCount; ++i)
			{
				_actionImpactLogics[i].ownerChampion = value;
				_actionImpactLogics[i].attackAction = this;
			}
		}
	}

	public AttackAction(AttackActionData attackActionData, AttackPerformanceData performanceData, AttackActionEffectData effectData)
	{
		_actionData = attackActionData;
		_impactData = new List<AttackImpactData>();

		TargetDecideKind impactRangeKind = (TargetDecideKind)_actionData.findTargetData.targetDecideKind;

		_decideTargetLogicContainerCount = (int)TargetDecideKind.End;
		_decideTargetLogicContainer = new AtkActionDecideTargetBase[_decideTargetLogicContainerCount];
		_allLogicsFindTargetsCache = new Champion[_decideTargetLogicContainerCount][];

		// 기본 타겟 찾는 로직 설정 및 찾은 타겟을 저장할 공간 미리 확보..
		baseFindTargetsCache = new Champion[10];
		_baseDecideTargetLogicIndex = (int)_actionData.findTargetData.targetDecideKind;

		CreateDecideTargetLogic(_actionData.findTargetData.targetDecideKind);

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

		_effectData = effectData;
	}

	public void AddImpactData(AttackImpactData impactData)
	{
		_impactData.Add(impactData);

		// Base 타겟 정하는 로직을 따르지 않고 개별적으로 찾기를 원한다면..
		if(impactData.isSeparateTargetFindLogic)
		{
			int index = (int)impactData.findTargetData.targetDecideKind;

			// 타겟 정하는 로직 생성 및 타겟을 저장할 공간 미리 확보..
			// 이미 생성되었다면 굳이 2번 생성할 필요 없으니 검사..
			if (null == _decideTargetLogicContainer[index])
			{
				CreateDecideTargetLogic(impactData.findTargetData.targetDecideKind);
			}
		}
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
		{
			if (null != _decideTargetLogicContainer[i])
				_decideTargetLogicContainer[i].OnStart();
		}

		_contPerf?.OnStart();

		targetChampion = ownerChampion.targetChampion;

		if (true == _effectData.isShowEffect)
		{
			ShowEffect(_effectData, _ownerChampion, targetChampion);
		}
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

		int findTargetCount = _decideTargetLogicContainer[_baseDecideTargetLogicIndex].FindTarget(_actionData.findTargetData, baseFindTargetsCache);
		for (int i = 0; i < findTargetCount; ++i)
		{
			int jLoopCount = _impactData.Count;
			for (int j = 0; j < jLoopCount; ++j)
			{
				_actionImpactLogics[(int)_impactData[j].mainData.kind].Impact(baseFindTargetsCache[i], _impactData[j]);
			}
		}
	}

	public void OnEnd()
	{
		for (int i = 0; i < _decideTargetLogicContainerCount; ++i)
		{
			if (null != _decideTargetLogicContainer[i])
				_decideTargetLogicContainer[i].OnEnd();
		}

		_contPerf?.OnEnd();

		targetChampion = null;
	}

	public void OnAnimationEndEvent()
	{
		_isEndAnimation = true;
	}

	private void CreateDecideTargetLogic(TargetDecideKind targetDecideKind)
	{
		int index = (int)targetDecideKind;

		switch (targetDecideKind)
		{
			case TargetDecideKind.OnlyTarget:
				_decideTargetLogicContainer[index] = new DecideTarget_OnlyTarget(this, _actionData);
				break;
			case TargetDecideKind.Range_Circle:
				_decideTargetLogicContainer[index] = new DecideTarget_InCircleRange(this, _actionData);
				break;
			case TargetDecideKind.Range_InTwoPointBox:
				_decideTargetLogicContainer[index] = new DecideTarget_InTwoPoint(this, _actionData);
				break;

			default:
				return;
		}

		_allLogicsFindTargetsCache[index] = new Champion[10];
	}

	public void ShowEffect(AttackActionEffectData effectData, Champion owner, Champion target)
	{
#if UNITY_EDITOR
		Debug.Assert(null != owner);
		Debug.Assert(null != target);
#endif

		Champion effectPointChampion = (effectData.effectPointKind == ActionStartPointKind.MyPosition) ? _ownerChampion : targetChampion;

		if (effectPointChampion.isDead)
			return;

		Vector3 effectStartPoint = effectPointChampion.transform.position;
		bool flipX = effectPointChampion.flipX;

		Effect effect = effectManager.GetEffect(effectData.showEffectName, effectStartPoint, flipX);

		Vector3 direction;
		effectPointChampion.blackboard.GetVectorValue(BlackboardKeyTable.moveDirection, out direction);
		effect.SetupAdditionalData(direction, effectPointChampion.transform);

		effect.gameObject.SetActive(true);

		effectPointChampion.AddActiveEffect(effect);
	}
}