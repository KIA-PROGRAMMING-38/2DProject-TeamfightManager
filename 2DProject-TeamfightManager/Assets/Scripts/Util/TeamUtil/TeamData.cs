using System.Collections.Generic;

[System.Serializable]
public class TeamData
{
	public string name;
}

[System.Serializable]
public class TeamBelongPilotData
{
	public int pilotCount;
	public List<string> pilotNameContainer;
}

[System.Serializable]
public class TeamResourceData
{
	public string logoImagePath;
}