using UnityEngine;

[CreateAssetMenu(fileName = "Container", menuName = "Test/DataContainer")]
public class TestScriptableObjectContainer : ScriptableObject
{
	public TestAttackActionScriptableObject[] allAtkActionPrefab;
	public TestChampionDataScriptableObject[] allChampionDataPrefab;
	public TestEffectDataScriptableObject[] allEffectDataPrefab;
	public TestPilotScriptableObject[] allPilotDataPrefab;
	public TestTeamDataScriptableObject[] allTeamDataPrefab;
}
