using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampInfoButtonUIManager : UIBase
{
	[SerializeField] private ChampInfoButtonUI _buttonPrefab;
	[SerializeField] private RectTransform _buttonParentTransform;
	[SerializeField] private ChampDetailInfoUI _champDetailInfoUI;

	private ChampInfoButtonUI[] _champInfoButtons;
	private int _champInfoButtonCount = 0;
	private int _curSelectButtonIndex = 0;

	public bool isShowDetailInfo { get => _champDetailInfoUI.gameObject.activeSelf; }

	private void Awake()
	{
		_champInfoButtonCount = s_dataTableManager.championDataTable.GetTotalChampionCount();
		_champInfoButtons = new ChampInfoButtonUI[_champInfoButtonCount];

		for (int i = 0; i < _champInfoButtonCount; ++i)
		{
			ChampInfoButtonUI newButton = Instantiate(_buttonPrefab, _buttonParentTransform);
			newButton.showChampName = s_dataTableManager.championDataTable.GetChampionName(i);
			newButton.index = i;
			newButton.manager = this;

			_champInfoButtons[i] = newButton;
		}
	}

	private void OnEnable()
	{
		_champDetailInfoUI.gameObject.SetActive(false);
	}

	// Champ Detail UI�� ��� �� ������ ��ư�� ������ �� ȣ��Ǵ� �ݹ� �Լ�..
	public void OnClickShowChampInfoButton(ChampInfoButtonUI button)
	{
		_champDetailInfoUI.gameObject.SetActive(true);
		_champDetailInfoUI.ShowChampionDetailInfo(button.showChampName);

		_curSelectButtonIndex = button.index;
	}

	// ���� �����ְ� �ִ� Champ Detail UI�� Hide�Ѵ�..
	public void HideDetailInfoUI()
	{
		_champDetailInfoUI.gameObject.SetActive(false);
	}

	// ���� �����ְ� �ִ� è�Ǿ��� ���� è�Ǿ��� Champ Detail UI�� �����ش�..
	public void OnClickShowPrevChampDetailInfo()
	{
		_curSelectButtonIndex = (_curSelectButtonIndex - 1 + _champInfoButtonCount) % _champInfoButtonCount;

		_champDetailInfoUI.gameObject.SetActive(true);
		_champDetailInfoUI.ShowChampionDetailInfo(_champInfoButtons[_curSelectButtonIndex].showChampName);
	}

	// ���� �����ְ� �ִ� è�Ǿ��� ���� è�Ǿ��� Champ Detail UI�� �����ش�..
	public void OnClickShowNextChampDetailInfo()
	{
		_curSelectButtonIndex = (_curSelectButtonIndex + 1) % _champInfoButtonCount;

		_champDetailInfoUI.gameObject.SetActive(true);
		_champDetailInfoUI.ShowChampionDetailInfo(_champInfoButtons[_curSelectButtonIndex].showChampName);
	}
}
