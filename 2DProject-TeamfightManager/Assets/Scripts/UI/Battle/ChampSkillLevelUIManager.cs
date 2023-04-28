using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ChampSkillLevelUI를 관리하고 파일럿의 정보를 받아와 ChampSkillLevelUI들에게 넘겨주는 역할.. 
/// </summary>
public class ChampSkillLevelUIManager : UIBase
{
    private ChampSkillLevelUI[] _champSkillLevelUI;
	private int _champSkillLevelUICount = 0;

	private void Awake()
	{
		_champSkillLevelUI = GetComponentsInChildren<ChampSkillLevelUI>();
		_champSkillLevelUICount = _champSkillLevelUI.Length;
	}

	// 파일럿의 챔피언 숙련도를 전부 가져와 하위 컴포넌트에게 넘겨준다..
	public void SetChampSkillLevelInfo(List<ChampionSkillLevelInfo> champSkillLevelInfoContainer)
	{
		for (int i = 0; i < _champSkillLevelUICount; ++i)
		{
			_champSkillLevelUI[i].gameObject.SetActive(false);
		}

		int loopCount = champSkillLevelInfoContainer.Count;
		for( int i = 0; i < loopCount; ++i)
		{
			_champSkillLevelUI[i].SetChampionSkillLevel(champSkillLevelInfoContainer[i]);
			_champSkillLevelUI[i].gameObject.SetActive(true);
		}
	}

	public void ShowSelectImage(int index)
	{
		_champSkillLevelUI[index].ShowSelectImage();
	}
}
