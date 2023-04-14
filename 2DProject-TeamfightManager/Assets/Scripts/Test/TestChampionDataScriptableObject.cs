using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChampionData", menuName = "Test/ChampionData")]
public class TestChampionDataScriptableObject : ScriptableObject
{
	public ChampionData championData;
	public ChampionStatus championStatus;
	public ChampionResourceData championResourceData;

	public string baseFilePath = "Assets/Data";
	public string extension = ".data";
}
