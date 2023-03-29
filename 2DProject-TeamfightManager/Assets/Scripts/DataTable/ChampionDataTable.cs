using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionDataTable
{
	private Dictionary<string, ChampionStatus> _champStatusContainer = new Dictionary<string, ChampionStatus>();
	private Dictionary<string, ChampionAnimData> _champAnimContainer = new Dictionary<string, ChampionAnimData>();

	public Dictionary<string, ChampionStatus> champStatusContainer { get => _champStatusContainer; }
	public Dictionary<string, ChampionAnimData> champAnimContainer { get => _champAnimContainer; }

	// Champion Status Getter Setter..
	public void AddChampionStatus(string championName, ChampionStatus championStatus)
	{
		champStatusContainer.Add(championName, championStatus);
	}
	public ChampionStatus GetChampionStatus(string championName)
	{
		return champStatusContainer[championName];
	}

	// Champion AnimData Getter Setter..
	public void AddChampionAnimData(string championName, ChampionAnimData championAnimData)
	{
		champAnimContainer.Add(championName, championAnimData);
	}
	public ChampionAnimData GetChampionAnimData(string championName)
	{
		return champAnimContainer[championName];
	}
}
