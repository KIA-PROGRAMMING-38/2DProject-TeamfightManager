using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 챔피언과 관련된 정보들을 저장하는 데이터 테이블..
/// </summary>
public class ChampionDataTable
{
	private Dictionary<string, ChampionData> _champDataContainer = new Dictionary<string, ChampionData>();
	private Dictionary<string, ChampionStatus> _champStatusContainer = new Dictionary<string, ChampionStatus>();
	private Dictionary<string, ChampionAnimData> _champAnimContainer = new Dictionary<string, ChampionAnimData>();

	private Dictionary<string, Sprite> _champImageContainer = new Dictionary<string, Sprite>();
	private Dictionary<string, Sprite> _skillIconContainer = new Dictionary<string, Sprite>();
	private Dictionary<string, Sprite> _ultimateIconContainer = new Dictionary<string, Sprite>();

	public Dictionary<string, ChampionStatus> champStatusContainer { get => _champStatusContainer; }
	public Dictionary<string, ChampionAnimData> champAnimContainer { get => _champAnimContainer; }

	// Champion Data Getter Setter..
	public void AddChampionData(string championName, ChampionData champData)
	{
		_champDataContainer.Add(championName, champData);
	}

	public ChampionData GetChampionData(string championName)
	{
		return _champDataContainer[championName];
	}

	// Image Getter Setter..
	public void AddChampionResourceData(string championName, ChampionResourceData championResourceData)
	{
		_champImageContainer.Add(championName, Resources.Load<Sprite>(championResourceData.champIconImagePath));
		_skillIconContainer.Add(championName, Resources.Load<Sprite>(championResourceData.skillIconImagePath));
		_ultimateIconContainer.Add(championName, Resources.Load<Sprite>(championResourceData.ultimateIconImagePath));
	}

	public Sprite GetChampionImage(string champName) => _champImageContainer[champName];
	public Sprite GetSkillIconImage(string champName) => _skillIconContainer[champName];
	public Sprite GetUltimateIconImage(string champName) => _ultimateIconContainer[champName];

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
