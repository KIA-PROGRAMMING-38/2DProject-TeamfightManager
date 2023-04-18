using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public TeamData data { get; private set; }
    private List<Pilot> _belongPilotContainer = new List<Pilot>();

	private BattleTeam _battleComponent;

	public void Awake()
	{
		_battleComponent = GetComponent<BattleTeam>();
	}

	public void SetNecessaryData(TeamData data, List<Pilot> belongPilots)
    {
        this.data = data;
        _belongPilotContainer = belongPilots;

        int loopCount = _belongPilotContainer.Count;
		for ( int i = 0; i < loopCount; ++i)
        {
            _belongPilotContainer[i].transform.parent = transform;
		}

		_battleComponent.belongPilot = _belongPilotContainer;
	}
}
