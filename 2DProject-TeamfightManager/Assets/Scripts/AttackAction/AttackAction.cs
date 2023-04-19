using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

/// <summary>
/// 챔피언의 공격 행동 로직을 수행하는 클래스..
/// </summary>
public class AttackAction
{
	public EffectManager effectManager { private get; set; }
	public ProjectileManager projectileManager
	{
		set
		{
			if (null != _summonSystem)
				_summonSystem.projectileManager = value;
		}
	}

	public Champion targetChampion { get; private set; }

	private AttackActionData _actionData;
	private AttackActionEffectData _effectData;
	private List<AttackImpactData> _impactData;
	private ActionImpactBase[] _actionImpactLogics;

	private ActionContinuousPerformance _contPerf;
	private AtkActionPassiveSystem _passiveSystem;
	private SummonSystem _summonSystem;

	private AtkActionDecideTargetBase[] _decideTargetLogicContainer;
	private int _baseDecideTargetLogicIndex;
	private int _decideTargetLogicContainerCount = 0;

	public Champion[] baseFindTargetsCache { get; private set; }
	private Champion[][] _allLogicsFindTargetsCache;

	private bool _isEndAction;
	public bool isEndAction
	{
		get => _isEndAction;
		private set
		{
			_isEndAction = value;

			if (true == _isEndAction)
			{
				ExecuteImpactLogic();
			}
		}
	}
	private bool _isEndAnimation;

	public bool isEnded { get => _isEndAnimation && _isEndAction; }

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
			if (null != _passiveSystem)
				_passiveSystem.ownerChampion = value;

			int loopCount = (int)AttackImpactEffectKind.End;
			for (int i = 0; i < loopCount; ++i)
			{
				_actionImpactLogics[i].ownerChampion = value;
				_actionImpactLogics[i].attackAction = this;
			}

			switch (_actionData.rangeType)
			{
				case AtkRangeType.FollowDefaultRange:
					attackRange = ownerChampion.status.range;
					break;
				case AtkRangeType.AllMapRange:
					attackRange = float.MaxValue;
					break;
				case AtkRangeType.CustomRange:
					attackRange = _actionData.atkRange;
					break;
			}

			if (null != _summonSystem)
				_summonSystem.ownerChampion = _ownerChampion;
		}
	}

	public float attackRange { get; private set; }

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

		// 패시브 행동인 경우 관련 로직 스크립트 생성..
		if (true == attackActionData.isPassive)
		{
			_passiveSystem = new AtkActionPassiveSystem(this, attackActionData.passiveData);
		}

		// 공격 시 효과 주는 것 관련 로직 스크립트 생성..
		_actionImpactLogics = new ActionImpactBase[(int)AttackImpactEffectKind.End];
		_actionImpactLogics[(int)AttackImpactEffectKind.Buff] = new Impact_Buff();
		_actionImpactLogics[(int)AttackImpactEffectKind.Debuff] = new Impact_Debuff();
		_actionImpactLogics[(int)AttackImpactEffectKind.Attack] = new Impact_AttackDamage();

		_effectData = effectData;

		if (true == attackActionData.isSummon)
		{
			_summonSystem = new SummonSystem(attackActionData.summonData, (Vector3 startPoint) =>
			{
				return _decideTargetLogicContainer[_baseDecideTargetLogicIndex].FindTarget(_actionData.findTargetData, baseFindTargetsCache, startPoint);
			}, 
			() =>
			{
				_decideTargetLogicContainer[(int)TargetDecideKind.OnlyTarget].FindTarget(_actionData.findTargetData, baseFindTargetsCache);
				return baseFindTargetsCache[0];
			});

			_summonSystem.atkImpactTeamKind = _actionData.findTargetData.targetTeamKind;
		}
	}

	public void AddImpactData(AttackImpactData impactData)
	{
		_impactData.Add(impactData);

		// Base 타겟 정하는 로직을 따르지 않고 개별적으로 찾기를 원한다면..
		if (impactData.isSeparateTargetFindLogic)
		{
			int index = (int)impactData.findTargetData.targetDecideKind;

			// 타겟 정하는 로직 생성 및 타겟을 저장할 공간 미리 확보..
			// 이미 생성되었다면 굳이 2번 생성할 필요 없으니 검사..
			if (null == _decideTargetLogicContainer[index])
			{
				CreateDecideTargetLogic(impactData.findTargetData.targetDecideKind);
			}
		}

		if (true == _actionData.isSummon && false == impactData.isSeparateTargetFindLogic)
		{
			_summonSystem.AddImpactData(impactData);
		}
	}

	public void RemoveImpactData(AttackImpactData impactData)
	{
		_impactData.Remove(impactData);
	}

	public void OnStart()
	{
		isEndAction = false;
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
		if (null == targetChampion)
		{
			isEndAction = true;
			return;
		}

		if (null != _contPerf)
		{
			_contPerf.OnUpdate();
			if (true == _contPerf.isEndPerformance)
			{
				isEndAction = true;
				_isEndAnimation = true;
			}
		}

		if(null != _summonSystem)
		{
			_summonSystem.Update();
		}
	}

	public void OnAction()
	{
		if (null == targetChampion)
		{
			isEndAction = true;
			return;
		}

		if (null == _contPerf)
		{
			isEndAction = true;
		}
		else
		{
			_contPerf.OnAction();
			isEndAction = _contPerf.isEndPerformance;
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

		Champion effectPointChampion = (effectData.effectPointKind == ActionStartPointKind.MyPosition) ? owner : target;

		if (effectPointChampion.isDead)
			return;

		Vector3 effectStartPoint = effectPointChampion.transform.position;
		bool flipX = effectPointChampion.flipX;

		Effect effect = effectManager.GetEffect(effectData.showEffectName, effectStartPoint, flipX);

		Vector3 direction;
		effectPointChampion.blackboard.GetVectorValue(BlackboardKeyTable.MOVE_DIRECTION, out direction);
		effect.SetupAdditionalData(direction, effectPointChampion.transform);

		effect.gameObject.SetActive(true);

		effectPointChampion.AddActiveEffect(effect);
	}

	public void ImpactTarget(AttackImpactData impactData, int targetCount, Champion[] targetArray)
	{
		// 찾은 타겟 개수만큼 효과 부여..
		for (int targetIndex = 0; targetIndex < targetCount; ++targetIndex)
		{
			_actionImpactLogics[(int)impactData.mainData.kind].Impact(targetArray[targetIndex], impactData);
		}
	}

	private void ExecuteImpactLogic()
	{
		if (_actionData.isSummon)
		{
			_summonSystem.OnStart(baseFindTargetsCache);
		}
		else
		{
			int findTargetCount = _decideTargetLogicContainer[_baseDecideTargetLogicIndex].FindTarget(_actionData.findTargetData, baseFindTargetsCache);

			int impactDataCount = _impactData.Count;
			for (int impactIndex = 0; impactIndex < impactDataCount; ++impactIndex)
			{
				AttackImpactData curImpactData = _impactData[impactIndex];

				int targetCount = findTargetCount;
				Champion[] targetArray = baseFindTargetsCache;

				// 만약 기본 타겟 찾는 로직을 사용하지 않는다면(다른 방법으로 타겟을 찾고 싶은 애들)..
				if (true == curImpactData.isSeparateTargetFindLogic)
				{
					int targetFindLogicIndex = (int)curImpactData.findTargetData.targetDecideKind;
					AtkActionDecideTargetBase curTargetFindLogic = _decideTargetLogicContainer[targetFindLogicIndex];
					targetCount = curTargetFindLogic.FindTarget(curImpactData.findTargetData, _allLogicsFindTargetsCache[targetFindLogicIndex]);
					targetArray = _allLogicsFindTargetsCache[targetFindLogicIndex];
				}

				// 찾은 타겟 개수만큼 효과 부여..
				ImpactTarget(curImpactData, targetCount, targetArray);
			}
		}
	}
}