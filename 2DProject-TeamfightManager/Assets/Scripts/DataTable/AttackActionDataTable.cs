using MH_AIFramework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AttackActionDataTable
{
	// 이펙트의 정보와 이펙트가 실행할 애니메이션을 이펙트 이름마다 저장할 컨테이너 생성..
	// 여기에는 처음 파일을 불러와 모든 이펙트의 정보를 저장할 예정..
	private Dictionary<string, AttackActionData> _actionDataContainer = new Dictionary<string, AttackActionData>();
	private Dictionary<string, AttackImpactData> _impactDataContainer = new Dictionary<string, AttackImpactData>();

	public void AddActionData(string actionName, AttackActionData actionData, AttackImpactData impactData)
	{
		_actionDataContainer.Add(actionName, actionData);
		_impactDataContainer.Add(actionName, impactData);
	}

	public AttackActionData GetActionData(string actionName)
	{
		return _actionDataContainer[actionName];
	}

	public AttackImpactData GetImpactData(string actionName)
	{
		return _impactDataContainer[actionName];
	}
}