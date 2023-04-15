using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 챔피언과 관련된 정보들을 저장하는 데이터 테이블..
/// </summary>
public class ChampionDataTable
{
	// 챔피언 생성에 필요한 데이터들을 저장하는 컨테이너..
	private Dictionary<string, ChampionData> _champDataContainer = new Dictionary<string, ChampionData>();
	private Dictionary<string, ChampionStatus> _champStatusContainer = new Dictionary<string, ChampionStatus>();
	private Dictionary<string, ChampionAnimData> _champAnimContainer = new Dictionary<string, ChampionAnimData>();

	private List<string> _champNameContainer = new List<string>();
	private int _totalChampCount = 0;

	// UI에서 사용할 리소스들을 저장하는 컨테이너..
	private Dictionary<string, Sprite> _champImageContainer = new Dictionary<string, Sprite>();
	private Dictionary<string, Sprite> _skillIconContainer = new Dictionary<string, Sprite>();
	private Dictionary<string, Sprite> _ultimateIconContainer = new Dictionary<string, Sprite>();

	// 챔피언 이름과 챔피언 생성에 필요한 데이터들을 받아와 컨테이너에 저장..
	public void AddChampionData(string championName, ChampionData champData, ChampionStatus champStatus, ChampionResourceData resourceData)
	{
		_champDataContainer.Add(championName, champData);
		_champStatusContainer.Add(championName, champStatus);

		_champNameContainer.Add(championName);

		++_totalChampCount;

		// UI에서 사용할 리소스들을 불러와 저장한다..
		_champImageContainer.Add(championName, Resources.Load<Sprite>(resourceData.champIconImagePath));
		_skillIconContainer.Add(championName, Resources.Load<Sprite>(resourceData.skillIconImagePath));
		_ultimateIconContainer.Add(championName, Resources.Load<Sprite>(resourceData.ultimateIconImagePath));
	}

	// 챔피언 이름에 맞는 데이터를 넘겨준다..
	public void GetChampionAllData(string championName, out ChampionData champData, out ChampionStatus champStatus, out ChampionAnimData animData)
	{
		champData = _champDataContainer[championName];
		champStatus = _champStatusContainer[championName];
		animData = _champAnimContainer[championName];
	}

	// UI에서 사용되는 리소스들 getter..
	public Sprite GetChampionImage(string champName) => _champImageContainer[champName];
	public Sprite GetSkillIconImage(string champName) => _skillIconContainer[champName];
	public Sprite GetUltimateIconImage(string champName) => _ultimateIconContainer[champName];
	public int GetTotalChampionCount() => _totalChampCount;
	public string GetChampionName(int index) => _champNameContainer[index];

	// Champion AnimData Getter Setter..
	public void AddChampionAnimData(string championName, ChampionAnimData championAnimData)
	{
		_champAnimContainer.Add(championName, championAnimData);
	}

	public ChampionAnimData GetChampionAnimData(string championName)
	{
		return _champAnimContainer[championName];
	}
}
