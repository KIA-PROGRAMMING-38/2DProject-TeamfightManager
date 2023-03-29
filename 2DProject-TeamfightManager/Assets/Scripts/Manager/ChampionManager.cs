using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChampionManager : MonoBehaviour
{
	public class MakeChampionData
	{
		public ChampionStatus ChampStatus;
		public ChampionAnimData ChampAnimData;
	}

	private GameManager _gameManager;
	private DataTableManager _dataTableManager;
	private ChampionDataTable _championDataTable;

	public GameManager gameManager
	{
		private get => _gameManager;
		set
		{
			_gameManager = value;

			_dataTableManager = gameManager.dataTableManager;
			_championDataTable = _dataTableManager.championDataTable;
		}
	}

	private Dictionary<string, MakeChampionData> _champions = new Dictionary<string, MakeChampionData>();

	private void Start()
	{
        
	}
}
