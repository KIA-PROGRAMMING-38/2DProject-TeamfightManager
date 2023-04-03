using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ChampionManager;

public class PilotManager : MonoBehaviour
{
	public class MakePilotData
	{
		public PilotData pilotData;
	}

	private GameManager _gameManager;
	private DataTableManager _dataTableManager;
	private PilotDataTable _pilotDataTable;
	private ChampionManager _championManager;

	public GameManager gameManager
	{
		private get => _gameManager;
		set
		{
			_gameManager = value;
			_dataTableManager = gameManager.dataTableManager;
			_championManager = gameManager.championManager;

			_pilotDataTable = _dataTableManager.pilotDataTable;
		}
	}

	private Pilot defaultPilotInstance;
	private Dictionary<string, MakePilotData> _pilots = new Dictionary<string, MakePilotData>();

	private void Start()
	{
		defaultPilotInstance = Resources.Load<Pilot>("Prefabs/Pilot");

		foreach (KeyValuePair<string, PilotData> elementPair in _pilotDataTable.pilotDataContainer)
		{
			MakePilotData newPilotMakeData =
				new MakePilotData { pilotData = elementPair.Value };

			_pilots[elementPair.Key] = newPilotMakeData;
		}
	}

	public Pilot GetPilotInstance(string pilotName)
	{
		Pilot newPilot = Instantiate<Pilot>(defaultPilotInstance);

		newPilot.data = _pilots[pilotName].pilotData;

		return newPilot;
	}
}
