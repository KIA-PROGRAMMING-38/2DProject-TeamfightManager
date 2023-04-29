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

    public enum SubMenuKind
    {
        ChampionBattleStatistics
    }

	private void Awake()
	{
		_background = transform.GetChild(0).gameObject;
		_champBattleStatisticsUI = GetComponentInChildren<ChampionBattleStatsticsUIManager>();

		_background.gameObject.SetActive(false);
		_champBattleStatisticsUI.gameObject.SetActive(false);
	}

	public void ShowSubMenu(SubMenuKind menuKind)
    {
		switch (menuKind)
		{
			case SubMenuKind.ChampionBattleStatistics:
				_champBattleStatisticsUI.gameObject.SetActive(true);
				break;

			default:
				return;
		}

		_background.SetActive(true);
	}

	public void CloseSubMenuUI()
    {
        OnCloseSubMenu?.Invoke();
    }
}
