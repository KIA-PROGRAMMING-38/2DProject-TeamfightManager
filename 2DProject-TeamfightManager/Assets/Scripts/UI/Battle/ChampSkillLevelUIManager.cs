using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampSkillLevelUIManager : UIBase
{
    private ChampSkillLevelUI[] _champSkillLevelUI;
	private int _champSkillLevelUICount = 0;

	private void Awake()
	{
		_champSkillLevelUI = GetComponentsInChildren<ChampSkillLevelUI>();
		_champSkillLevelUICount = _champSkillLevelUI.Length;
	}

	private void Start()
	{
		for( int i = 0; i < _champSkillLevelUICount; ++i)
		{
			_champSkillLevelUI[i].gameObject.SetActive(false);
		}
	}

	public void SetChampSkillLevelInfo(List<ChampionSkillLevelInfo> champSkillLevelInfoContainer)
	{
		int loopCount = champSkillLevelInfoContainer.Count;
		for( int i = 0; i < loopCount; ++i)
		{
			_champSkillLevelUI[i].SetChampionSkillLevel(champSkillLevelInfoContainer[i]);
			_champSkillLevelUI[i].gameObject.SetActive(true);
		}
	}
}
