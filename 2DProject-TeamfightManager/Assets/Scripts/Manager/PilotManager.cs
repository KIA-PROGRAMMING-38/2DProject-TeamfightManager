using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 파일럿을 관리하는 매니저 클래스..
/// </summary>
public class PilotManager : MonoBehaviour
{
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

	// 외부에서 파일럿의 인스턴스를 받아오고 싶을 때 호출되는 함수..
	public Pilot GetPilotInstance(string pilotName)
	{
		Pilot newPilot = Instantiate<Pilot>(_pilotDataTable.DefaultPilotPrefab);

		newPilot.Initialize(_pilotDataTable.GetPilotData(pilotName));

		return newPilot;
	}
}
