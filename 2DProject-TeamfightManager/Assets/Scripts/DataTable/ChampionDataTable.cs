using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// è�Ǿ�� ���õ� �������� �����ϴ� ������ ���̺�..
/// </summary>
public class ChampionDataTable
{
	// è�Ǿ� ������ �ʿ��� �����͵��� �����ϴ� �����̳�..
	private Dictionary<string, ChampionData> _champDataContainer = new Dictionary<string, ChampionData>();
	private Dictionary<string, ChampionStatus> _champStatusContainer = new Dictionary<string, ChampionStatus>();
	private Dictionary<string, ChampionAnimData> _champAnimContainer = new Dictionary<string, ChampionAnimData>();

	private List<string> _champNameContainer = new List<string>();
	private int _totalChampCount = 0;

	// UI���� ����� ���ҽ����� �����ϴ� �����̳�..
	private Dictionary<string, Sprite> _champImageContainer = new Dictionary<string, Sprite>();
	private Dictionary<string, Sprite> _skillIconContainer = new Dictionary<string, Sprite>();
	private Dictionary<string, Sprite> _ultimateIconContainer = new Dictionary<string, Sprite>();

	// è�Ǿ� �̸��� è�Ǿ� ������ �ʿ��� �����͵��� �޾ƿ� �����̳ʿ� ����..
	public void AddChampionData(string championName, ChampionData champData, ChampionStatus champStatus, ChampionResourceData resourceData)
	{
		_champDataContainer.Add(championName, champData);
		_champStatusContainer.Add(championName, champStatus);

		_champNameContainer.Add(championName);

		++_totalChampCount;

		// UI���� ����� ���ҽ����� �ҷ��� �����Ѵ�..
		_champImageContainer.Add(championName, Resources.Load<Sprite>(resourceData.champIconImagePath));
		_skillIconContainer.Add(championName, Resources.Load<Sprite>(resourceData.skillIconImagePath));
		_ultimateIconContainer.Add(championName, Resources.Load<Sprite>(resourceData.ultimateIconImagePath));
	}

	// è�Ǿ� �̸��� �´� �����͸� �Ѱ��ش�..
	public void GetChampionAllData(string championName, out ChampionData champData, out ChampionStatus champStatus, out ChampionAnimData animData)
	{
		champData = _champDataContainer[championName];
		champStatus = _champStatusContainer[championName];
		animData = _champAnimContainer[championName];
	}

	public ChampionData GetChampionData(string championName) => _champDataContainer[championName];
	public ChampionStatus GetChampionStatus(string championName) => _champStatusContainer[championName];

	// UI���� ���Ǵ� ���ҽ��� getter..
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

	public ChampionAnimData GetChampionAnimData(string championName) => _champAnimContainer[championName];
}
