using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TeamDataTable
{
	public const string TEAM_PREFAB_FILEPATH = "Prefabs/Team";

	public Dictionary<string, TeamData> _teamDataContainer = new Dictionary<string, TeamData>();
	public Dictionary<string, TeamBelongPilotData> _belongPilotDataContainer = new Dictionary<string, TeamBelongPilotData>();
	public Dictionary<string, Sprite> _teamLogoSpriteContainer = new Dictionary<string, Sprite>();

	public List<string> _allTeamNameContainer = new List<string>();

	public int GetTotalTeamCount() => _allTeamNameContainer.Count;
	public string GetTeamName(int index) => _allTeamNameContainer[index];

	public void AddTeamInfo(TeamData data, TeamBelongPilotData belongPilotData, TeamResourceData resourceData)
	{
		_teamDataContainer.Add(data.name, data);
		_belongPilotDataContainer.Add(data.name, belongPilotData);

		_teamLogoSpriteContainer.Add(data.name, Resources.Load<Sprite>(resourceData.logoImagePath));

		_allTeamNameContainer.Add(data.name);
	}

	public bool GetTeamInfo(string teamName, out TeamData getData, out TeamBelongPilotData getBelongPilotData)
	{
		if(false == _teamDataContainer.TryGetValue(teamName, out getData))
		{
			getBelongPilotData = null;
			return false;
		}

		getBelongPilotData = _belongPilotDataContainer[teamName];

		return true;
	}

	public Sprite GetLogoSprite(string teamName)
	{
#if UNITY_EDITOR
		Debug.Assert(_teamLogoSpriteContainer.ContainsKey(teamName));
#endif

		return _teamLogoSpriteContainer[teamName];
	}
}