using System.Collections.Generic;

/// <summary>
/// 챔피언과 관련된 정보들을 저장하는 데이터 테이블..
/// </summary>
public class ChampionDataTable
{
	private Dictionary<string, ChampionStatus> _champStatusContainer = new Dictionary<string, ChampionStatus>();
	private Dictionary<string, ChampionAnimData> _champAnimContainer = new Dictionary<string, ChampionAnimData>();

	public Dictionary<string, ChampionStatus> champStatusContainer { get => _champStatusContainer; }
	public Dictionary<string, ChampionAnimData> champAnimContainer { get => _champAnimContainer; }

	// Champion Status Getter Setter..
	public void AddChampionStatus(string championName, ChampionStatus championStatus)
	{
		champStatusContainer.Add(championName, championStatus);
	}
	public ChampionStatus GetChampionStatus(string championName)
	{
		return champStatusContainer[championName];
	}

	// Champion AnimData Getter Setter..
	public void AddChampionAnimData(string championName, ChampionAnimData championAnimData)
	{
		champAnimContainer.Add(championName, championAnimData);
	}
	public ChampionAnimData GetChampionAnimData(string championName)
	{
		return champAnimContainer[championName];
	}
}
