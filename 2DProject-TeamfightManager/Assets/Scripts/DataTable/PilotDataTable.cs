using System.Collections.Generic;

/// <summary>
/// 파일럿과 관련된 정보들을 저장하는 데이터 테이블..
/// </summary>
public class PilotDataTable
{
	private Dictionary<string, PilotData> _pilotDataContainer = new Dictionary<string, PilotData>();

	public Dictionary<string, PilotData> pilotDataContainer { get => _pilotDataContainer; }

	// Champion Status Getter Setter..
	public void AddPilotData(string championName, PilotData championStatus)
	{
		pilotDataContainer.Add(championName, championStatus);
	}
	public PilotData GetPilotData(string championName)
	{
		return pilotDataContainer[championName];
	}
}
