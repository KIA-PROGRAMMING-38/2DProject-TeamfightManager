using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGlobalData", menuName = "Config/GameGlobalData")]
public class GameGlobalData : ScriptableObject
{
	public int PilotCount = 4;
	public float battleFightTime = 60f;

	public string pilotGlobalFilePath;

	public string pilotDirectoryName;
	public string championDirectoryName;
	public string attackActionDirectoryName;
	public string effectDirectoryName;

	public List<string> testRedChampionCreateOrder;
	public List<string> testRedPilotCreateOrder;

	public List<string> testBlueChampionCreateOrder;
	public List<string> testBluePilotCreateOrder;
}