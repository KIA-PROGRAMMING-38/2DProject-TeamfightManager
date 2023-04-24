using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Util.Pool;

/// <summary>
/// ���Ϸ��� �����ϴ� �Ŵ��� Ŭ����..
/// </summary>
public class PilotManager : MonoBehaviour
{
	private GameManager _gameManager;
	private DataTableManager _dataTableManager;
	private PilotDataTable _pilotDataTable;
	private ChampionManager _championManager;

	private ObjectPooler<Pilot> _pilotPooler;

	public GameManager gameManager
	{
		private get => _gameManager;
		set
		{
			_gameManager = value;
			_dataTableManager = gameManager.dataTableManager;
			_championManager = gameManager.championManager;

			_pilotDataTable = _dataTableManager.pilotDataTable;

			_pilotPooler = new ObjectPooler<Pilot>(CreatePilot, null, null, null, 8, 8, 1000);
		}
	}

	private Pilot CreatePilot()
	{
		Pilot newPilot = Instantiate<Pilot>(_pilotDataTable.DefaultPilotPrefab);

		DontDestroyOnLoad(newPilot.gameObject);

		newPilot.pilotManager = this;
		newPilot.transform.parent = transform;
		newPilot.gameObject.SetActive(false);

		return newPilot;
	}

	// �ܺο��� ���Ϸ��� �ν��Ͻ��� �޾ƿ��� ���� �� ȣ��Ǵ� �Լ�..
	public Pilot GetPilotInstance(string pilotName)
	{
		Pilot newPilot = _pilotPooler.Get();

		newPilot.transform.parent = null;
		newPilot.Initialize(_pilotDataTable.GetPilotData(pilotName));

		newPilot.gameObject.SetActive(true);

		return newPilot;
	}

	public void ReleasePilot(Pilot pilot)
	{
		_pilotPooler.Release(pilot);

		pilot.transform.parent = transform;
		pilot.gameObject.SetActive(false);
	}
}
