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

	public Champion _championDefaultPrefab;
	private Dictionary<string, MakeChampionData> _champions = new Dictionary<string, MakeChampionData>();

	private void Start()
	{
        _championDefaultPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Champion.prefab").GetComponent<Champion>();

		foreach (KeyValuePair<string, ChampionStatus> elementPair in _championDataTable.champStatusContainer)
        {
            MakeChampionData newMakeChampData =
                new MakeChampionData { ChampStatus = elementPair.Value, ChampAnimData = _championDataTable.GetChampionAnimData(elementPair.Key) };

            _champions[elementPair.Key] = newMakeChampData;
		}
	}

	public void AddChampionMakeData(string championName, in MakeChampionData makeChampionData)
    {
        _champions.Add(championName, makeChampionData);
    }

    public Champion GetChampionInstance(string championName)
    {
        Champion newChampion = Instantiate<Champion>(_championDefaultPrefab);

		MakeChampionData makeChampData = _champions[championName];
        newChampion.status = makeChampData.ChampStatus;
        newChampion.animData = makeChampData.ChampAnimData;

		return newChampion;
	}
}
