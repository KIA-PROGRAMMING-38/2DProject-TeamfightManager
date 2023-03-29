using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotDataTable
{
	private Dictionary<string, PilotData> _pilotDataContainer = new Dictionary<string, PilotData>();

	public Dictionary<string, PilotData> pilotDataContainer { get => _pilotDataContainer; }

	// Champion Status Getter Setter..
	public void AddPilotData(string championName, PilotData championStatus)
	{
		pilotDataContainer.Add(championName, championStatus);
	}
	public PilotData GetPilotData(string championName)
	{
		return pilotDataContainer[championName];
	}
}
