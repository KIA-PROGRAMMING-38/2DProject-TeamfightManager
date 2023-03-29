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

	private Dictionary<string, MakePilotData> _pilots = new Dictionary<string, MakePilotData>();

	private void Start()
	{

	}
}
