using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTableManager : MonoBehaviour
{
	public ChampionDataTable championDataTable { get; private set; }
	public PilotDataTable pilotDataTable { get; private set; }

	private void Awake()
	{
		championDataTable = new ChampionDataTable();
		pilotDataTable = new PilotDataTable();
	}
}
