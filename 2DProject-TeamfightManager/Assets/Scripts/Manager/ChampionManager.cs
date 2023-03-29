using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChampionManager : MonoBehaviour
{
    public class MakeChampionData
    {

    }

	private GameManager _gameManager;
    private DataTableManager _dataTableManager;

	public GameManager gameManager
    {
        private get => _gameManager;
        set
        {
            _gameManager = value;

			_dataTableManager = gameManager.dataTableManager;
		}
    }

	private Dictionary<string, MakeChampionData> _champions = new Dictionary<string, MakeChampionData>();

	private void Start()
	{
        
	}
}
