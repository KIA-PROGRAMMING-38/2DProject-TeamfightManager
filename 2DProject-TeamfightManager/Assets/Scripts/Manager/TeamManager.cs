using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 팀을 관리하는 매니저 클래스..
/// </summary>
public class TeamManager : MonoBehaviour
{
	public GameManager gameManager
	{
		private get => _gameManager;
		set
		{
			_gameManager = value;

			_dataTableManager = value.dataTableManager;
			_pilotManager = value.pilotManager;

			_teamDataTable = _dataTableManager.teamDataTable;

			SetupTeam();
		}
	}

	private GameManager _gameManager;
	public DataTableManager _dataTableManager;
	public PilotManager _pilotManager;

	private Team _teamPrefab;
	private TeamDataTable _teamDataTable;
	private Dictionary<string, Team> _teamContainer;

	private void SetupTeam()
	{
		_teamPrefab = Resources.Load<Team>(TeamDataTable.TEAM_PREFAB_FILEPATH);

		int teamCount = _teamDataTable.GetTotalTeamCount();
		_teamContainer = new Dictionary<string, Team>(teamCount);
		for (int i = 0; i < teamCount; ++i)
		{
			Team newTeam = Instantiate(_teamPrefab, transform);

			TeamData data;
			TeamBelongPilotData belongPilotData;

			string teamName = _teamDataTable.GetTeamName(i);
			_teamDataTable.GetTeamInfo(teamName, out data, out belongPilotData);

			List<Pilot> pilots = CreateBelongPilots(belongPilotData);

			newTeam.SetNecessaryData(data, pilots);

			_teamContainer.Add(teamName, newTeam);
		}
	}

	private List<Pilot> CreateBelongPilots(TeamBelongPilotData belongPilotData)
	{
		List<Pilot> pilots = new List<Pilot>();

		for( int i = 0; i <  belongPilotData.pilotCount; ++i)
			pilots.Add(_pilotManager.GetPilotInstance(belongPilotData.pilotNameContainer[i]));

		return pilots;
	}

	public Team GetTeamInstance(string teamName)
	{
		return _teamContainer[teamName];
	}
}
