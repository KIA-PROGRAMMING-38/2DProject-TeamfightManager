using UnityEngine;

[CreateAssetMenu(fileName = "GameGlobalData", menuName = "Config/GameGlobalData")]
public class GameGlobalData : ScriptableObject
{
	public int PilotCount = 4;

	public string pilotGlobalFilePath;

	public string pilotDirectoryName;
	public string championDirectoryName;
	public string attackActionDirectoryName;
	public string effectDirectoryName;
}