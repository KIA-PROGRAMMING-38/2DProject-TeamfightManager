using MH_AIFramework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AttackActionDataTable
{
	// 이펙트의 정보와 이펙트가 실행할 애니메이션을 이펙트 이름마다 저장할 컨테이너 생성..
	// 여기에는 처음 파일을 불러와 모든 이펙트의 정보를 저장할 예정..
	private List<(AttackActionData actionData, List<AttackImpactData> impactDatas)> _dataContainer = 
		new List<(AttackActionData actionData, List<AttackImpactData> impactDatas)>();

	public int actionCount
	{
		set
		{
			_dataContainer.Capacity = value;
			for( int i = 0; i < value; ++i)
			{
				_dataContainer.Add(new(null, null));
			}
		}
	}

	public void AddActionData(AttackActionData actionData, List<AttackImpactData> impactData)
	{
		_dataContainer[actionData.uniqueKey] = new(actionData, impactData);
	}

	public AttackAction GetAttackAction(int uniqueKey)
	{
#if UNITY_EDITOR
		Debug.Assert(uniqueKey < _dataContainer.Count, "AttackActionDataTable's GetAttackAction() : Index out of range");
#endif

		AttackAction attackAction = new AttackAction(_dataContainer[uniqueKey].actionData);

		int loopCount = _dataContainer[uniqueKey].impactDatas.Count;
		for( int i = 0; i < loopCount; ++i)
		{
			attackAction.AddImpactData(_dataContainer[uniqueKey].impactDatas[i]);
		}

		return attackAction;
	}
}