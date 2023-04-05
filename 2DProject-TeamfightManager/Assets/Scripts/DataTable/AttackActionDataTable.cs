using MH_AIFramework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AttackActionDataTable
{
	// 이펙트의 정보와 이펙트가 실행할 애니메이션을 이펙트 이름마다 저장할 컨테이너 생성..
	// 여기에는 처음 파일을 불러와 모든 이펙트의 정보를 저장할 예정..
	private Dictionary<int, AttackActionData> _actionDataContainer = new Dictionary<int, AttackActionData>();
	private Dictionary<int, List<AttackImpactData>> _impactDataContainer = new Dictionary<int, List<AttackImpactData>>();

	public void AddActionData(AttackActionData actionData, List<AttackImpactData> impactData)
	{
		_actionDataContainer.Add(actionData.uniqueKey, actionData);
		_impactDataContainer.Add(actionData.uniqueKey, impactData);
	}

	public AttackActionData GetActionData(int uniqueKey)
	{
		return _actionDataContainer[uniqueKey];
	}

	public List<AttackImpactData> GetImpactData(int uniqueKey)
	{
		return _impactDataContainer[uniqueKey];
	}
}