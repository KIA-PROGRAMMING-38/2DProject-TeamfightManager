using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ChampionBattleStatsticsUI : UIBase
{
	public string showChampionName
	{
		set
		{
			Sprite champIconSprite = s_dataTableManager.championDataTable.GetChampionImage(value);
			_champIconImage.sprite = champIconSprite;
			UIUtility.CalcSpriteCenterPos(_champIconImage.rectTransform, champIconSprite, _champIconImage.rectTransform.localPosition);

			_champNameText.text = value;

			ChampionBattleStatistics champBattleStatistics = s_dataTableManager.statisticsDataTable.GetChampBattleStatistics(value);
			SetupChampBattleStatistics(champBattleStatistics);
		}
	}

	public int ranking
	{
		set
		{
			_rankingText.text = StringTable.GetString(value);
		}
	}

	private Image _champIconImage;

	private TMP_Text _rankingText;
	private TMP_Text _champNameText;
	private TMP_Text _winRateText;
	private TMP_Text _pickCounText;
	private TMP_Text _banCountText;
	private TMP_Text _banpickRateText;
	private TMP_Text _totalAtkDamageText;
	private TMP_Text _totalTakeDamageText;
	private TMP_Text _totalHealText;

	private void Awake()
	{
		_champIconImage = transform.Find("IconImageMask").Find("ChampIconImage").GetComponent<Image>();

		_rankingText = transform.Find("RankingText").GetComponent<TMP_Text>();
		_champNameText = transform.Find("ChampNameText").GetComponent<TMP_Text>();
		_winRateText = transform.Find("WinRateText").GetComponent<TMP_Text>();
		_pickCounText = transform.Find("PickCountText").GetComponent<TMP_Text>();
		_banCountText = transform.Find("BanCountText").GetComponent<TMP_Text>();
		_banpickRateText = transform.Find("BanpickRateText").GetComponent<TMP_Text>();
		_totalAtkDamageText = transform.Find("TotalAtkDamageText").GetComponent<TMP_Text>();
		_totalTakeDamageText = transform.Find("TotalTakeDamageText").GetComponent<TMP_Text>();
		_totalHealText = transform.Find("TotalHealText").GetComponent<TMP_Text>();
	}

	private void SetupChampBattleStatistics(ChampionBattleStatistics battleStatistics)
	{
		int totalBanpickCount = battleStatistics.totalBanCount + battleStatistics.totalPickCount;

		double winRate = 0.0;
		double banpickRate = 0.0;
		if (totalBanpickCount > 0)
		{
			winRate = battleStatistics.totalWinCount / (double)totalBanpickCount * 100.0;
			banpickRate = s_dataTableManager.statisticsDataTable.totalBattleDayCount / (double)totalBanpickCount * 100.0;
		}

		_winRateText.text = StringTable.GetString(Math.Round(winRate, 2)) + "%";
		_pickCounText.text = StringTable.GetString(battleStatistics.totalPickCount);
		_banCountText.text = StringTable.GetString(battleStatistics.totalBanCount);
		_banpickRateText.text = StringTable.GetString(Math.Round(banpickRate, 2)) + "%";
		_totalAtkDamageText.text = StringTable.GetString(battleStatistics.totalAtkDamageAmount);
		_totalTakeDamageText.text = StringTable.GetString(battleStatistics.totalTakeDamageAount);
		_totalHealText.text = StringTable.GetString(battleStatistics.totalHealAmount);
	}
}
