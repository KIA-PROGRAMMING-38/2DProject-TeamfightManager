using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackActionData", menuName = "Test/AttackActionData")]
public class TestAttackActionScriptableObject : ScriptableObject
{
	public string actionName;
	public string extension = ".data";
	public string baseFilePath = "Assets/Data";

	public AttackActionData actionData;
	public List<AttackImpactData> impactData;
	public AttackPerformanceData performanceData;

	public AttackImpactEffectKind kindEnum;

	public AttackImpactType atkImpactEnum;
	public BuffImpactType buffEnum;
	public DebuffImpactType debuffEnum;

	public MovePerformanceType movePerformanceEnum;
}