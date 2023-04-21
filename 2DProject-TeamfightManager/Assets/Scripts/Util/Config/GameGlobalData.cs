using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGlobalData", menuName = "Config/GameGlobalData")]
public class GameGlobalData : ScriptableObject
{
	public int PilotCount = 4;
	public float battleFightTime = 60f;

	public string DefaultRedTeamName;
	public string DefaultBlueTeamName;

    public string pilotGlobalFilePath;

	public string pilotFileName;
	public string championFileName;
	public string attackActionDataFileName;
	public string attackActionPerfDataFileName;
	public string attackActionImpactDataFileName;
	public string effectFileName;
	public string teamFileName;

    public List<string> testRedChampionCreateOrder;
	public List<string> testRedPilotCreateOrder;

	public List<string> testBlueChampionCreateOrder;
	public List<string> testBluePilotCreateOrder;

	public BanpickStageGlobalData banpickStageGlobalData;
	public SpawnObjectGlobalData spawnObjectGolbalData;

	public Sprite pilotTrunkSprite;
	public Sprite[] pilotHairSprite;
}