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
	private GameObject _closeButtonObject;

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
		_closeButtonObject = transform.Find("HideDetailInfoButton").gameObject;

		// 얘네는 서브 버튼 그룹의 버튼이 눌렸을 때 보여줘야하는 애들이기 때문에 시작할 때 꺼두기..
		_background.gameObject.SetActive(false);
		_champBattleStatisticsUI.gameObject.SetActive(false);
		_champInfoUI.gameObject.SetActive(false);
		_closeButtonObject.SetActive(false);
	}

	// 어떤 UI를 출력할지 받아와 그 UI를 출력..
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

		_closeButtonObject.SetActive(true);
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
					return;
				}
				else
				{
					_champInfoUI.gameObject.SetActive(false);
				}

				break;
		}

		_background.SetActive(false);
		_closeButtonObject.SetActive(false);
		OnCloseSubMenu?.Invoke();
	}
}
