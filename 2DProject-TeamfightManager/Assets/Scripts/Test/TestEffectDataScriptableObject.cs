using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Test/EffecetData")]
public class TestEffectDataScriptableObject : ScriptableObject
{
	public EffectData effectData;

	public string baseFilePath = "Assets/Data";
	public string extension = ".data";
}