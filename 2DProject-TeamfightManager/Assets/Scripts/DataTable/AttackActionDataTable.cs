using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 행동을 생성할 때 필요한 정보를 가지고 있는 데이터 테이블..
/// </summary>
public class AttackActionDataTable
{
	// 이펙트의 정보와 이펙트가 실행할 애니메이션을 이펙트 이름마다 저장할 컨테이너 생성..
	// 여기에는 처음 파일을 불러와 모든 이펙트의 정보를 저장할 예정..
	private List<AttackActionData> _actionDataContainer = new List<AttackActionData>();
	private List<List<AttackImpactData>> _impactDataContainer = new List<List<AttackImpactData>>();
	private List<AttackPerformanceData> _performanceDataContinaer = new List<AttackPerformanceData>();
	private List<AttackActionEffectData> _effectDataContainer = new List<AttackActionEffectData>();

	public int actionCount
	{
		set
		{
			_actionDataContainer.Capacity = value;
			_impactDataContainer.Capacity = value;
			_performanceDataContinaer.Capacity = value;
			for ( int i = 0; i < value; ++i)
			{
				_actionDataContainer.Add(null);
				_impactDataContainer.Add(null);
				_performanceDataContinaer.Add(null);
				_effectDataContainer.Add(null);
			}
		}
	}

	// 공격 행동 데이터를 받아와 컨테이너에 추가한다..
	public void AddActionData(AttackActionData actionData, List<AttackImpactData> impactData, AttackPerformanceData performanceData,
		AttackActionEffectData effectData)
	{
		_actionDataContainer[actionData.uniqueKey] = actionData;
		_impactDataContainer[actionData.uniqueKey] = impactData;
		_performanceDataContinaer[actionData.uniqueKey] = performanceData;
		_effectDataContainer[actionData.uniqueKey] = effectData;
	}

	// 인자로 받은 인덱스 값을 컨테이너에서 가져와 AttackAction을 만들고 초기화한 뒤 반환한다..
	public AttackAction GetAttackAction(int uniqueKey)
	{
#if UNITY_EDITOR
		Debug.Assert(uniqueKey < _actionDataContainer.Count, "AttackActionDataTable's GetAttackAction() : Index out of range");
#endif

		AttackActionData actionData = _actionDataContainer[uniqueKey];
		AttackPerformanceData performanceData = _performanceDataContinaer[uniqueKey];
		List<AttackImpactData> impactData = _impactDataContainer[uniqueKey];
		AttackActionEffectData effectData = _effectDataContainer[uniqueKey];

		AttackAction newAction = new AttackAction(actionData, performanceData, effectData);

		int loopCount = impactData.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			newAction.AddImpactData(impactData[i]);
		}

		return newAction;
	}
}