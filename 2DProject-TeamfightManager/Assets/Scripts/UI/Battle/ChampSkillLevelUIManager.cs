using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ChampSkillLevelUI�� �����ϰ� ���Ϸ��� ������ �޾ƿ� ChampSkillLevelUI�鿡�� �Ѱ��ִ� ����.. 
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

	// ���Ϸ��� è�Ǿ� ���õ��� ���� ������ ���� ������Ʈ���� �Ѱ��ش�..
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
