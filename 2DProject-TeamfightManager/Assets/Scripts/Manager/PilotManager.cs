using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ���Ϸ��� �����ϴ� �Ŵ��� Ŭ����..
/// </summary>
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
		foreach (KeyValuePair<string, PilotData> elementPair in _pilotDataTable.pilotDataContainer)
		{
			MakePilotData newPilotMakeData =
				new MakePilotData { pilotData = elementPair.Value };

			_pilots[elementPair.Key] = newPilotMakeData;
		}
	}

	// �ܺο��� ���Ϸ��� �ν��Ͻ��� �޾ƿ��� ���� �� ȣ��Ǵ� �Լ�..
	public Pilot GetPilotInstance(string pilotName)
	{
		Pilot newPilot = Instantiate<Pilot>(_pilotDataTable.DefaultPilotPrefab);

		newPilot.Initialize(_pilots[pilotName].pilotData);

		return newPilot;
	}
}
