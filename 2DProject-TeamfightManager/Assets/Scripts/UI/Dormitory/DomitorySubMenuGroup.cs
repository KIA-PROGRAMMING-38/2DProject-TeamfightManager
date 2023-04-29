using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DomitorySubMenuGroup : MonoBehaviour
{
    public event Action OnCloseSubMenu;

	private GameObject _background;
    private ChampionBattleStatsticsUIManager _champBattleStatisticsUI;
	private ChampInfoButtonUIManager _champInfoUI;
	private SubMenuKind _curShowMenuKind;

    public enum SubMenuKind
    {
        ChampionBattleStatistics,
		ChampionInfomation
    }

	private void Awake()
	{
		_background = transform.GetChild(0).gameObject;
		_champBattleStatisticsUI = GetComponentInChildren<ChampionBattleStatsticsUIManager>();
		_champInfoUI = GetComponentInChildren<ChampInfoButtonUIManager>();

		// ��״� ���� ��ư �׷��� ��ư�� ������ �� ��������ϴ� �ֵ��̱� ������ ������ �� ���α�..
		_background.gameObject.SetActive(false);
		_champBattleStatisticsUI.gameObject.SetActive(false);
		_champInfoUI.gameObject.SetActive(false);
	}

	// � UI�� ������� �޾ƿ� �� UI�� ���..
	public void ShowSubMenu(SubMenuKind menuKind)
    {
		switch (menuKind)
		{
			case SubMenuKind.ChampionBattleStatistics:
				_champBattleStatisticsUI.gameObject.SetActive(true);
				break;

			case SubMenuKind.ChampionInfomation:
				_champInfoUI.gameObject.SetActive(true);
				break;

			default:
				return;
		}

		_curShowMenuKind = menuKind;
		_background.SetActive(true);
	}

	public void CloseSubMenuUI()
    {
		switch (_curShowMenuKind)
		{
			case SubMenuKind.ChampionBattleStatistics:
				_champBattleStatisticsUI.gameObject.SetActive(false);
				break;
			case SubMenuKind.ChampionInfomation:
				if(true == _champInfoUI.isShowDetailInfo)
				{
					_champInfoUI.HideDetailInfoUI();
				}

				break;
		}

		OnCloseSubMenu?.Invoke();
	}
}
