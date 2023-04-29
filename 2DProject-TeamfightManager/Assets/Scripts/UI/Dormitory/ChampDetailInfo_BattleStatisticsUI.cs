using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChampDetailInfo_BattleStatisticsUI : UIBase
{
    private TMP_Text _totalBattleGameCountText;
    private TMP_Text _totalWinCountText;
    private TMP_Text _totalLoseCountText;
	private TMP_Text _winRateText;
	private TMP_Text _pickCounText;
	private TMP_Text _banCountText;
	private TMP_Text _banpickRateText;
	private TMP_Text _totalAtkDamageText;
	private TMP_Text _totalTakeDamageText;
	private TMP_Text _totalHealText;

	// ����ؾ��� è�Ǿ� �̸��� �޾ƿ� UI �ʱ�ȭ..
	public string showChampName
    {
        set
        {
            // è�Ǿ� ��� ������ ������ �װſ� �°� UI �ʱ�ȭ ����..
			ChampionBattleStatistics champBattleStatistics = s_dataTableManager.statisticsDataTable.GetChampBattleStatistics(value);

			SetupChampBattleStatistics(champBattleStatistics);
        }
    }

	void Awake()
    {
        _totalBattleGameCountText = transform.Find("TotalBattleGameCountText").GetComponent<TMP_Text>();
		_totalWinCountText = transform.Find("WinCountText").GetComponent<TMP_Text>();
		_totalLoseCountText = transform.Find("LoseCountText").GetComponent<TMP_Text>();
		_winRateText = transform.Find("WinRateText").GetComponent<TMP_Text>();
		_pickCounText = transform.Find("PickCountText").GetComponent<TMP_Text>();
		_banCountText = transform.Find("BanCountText").GetComponent<TMP_Text>();
		_banpickRateText = transform.Find("BanpickRateText").GetComponent<TMP_Text>();
		_totalAtkDamageText = transform.Find("TotalAtkDamageText").GetComponent<TMP_Text>();
		_totalTakeDamageText = transform.Find("TotalTakeDamageText").GetComponent<TMP_Text>();
		_totalHealText = transform.Find("TotalHealText").GetComponent<TMP_Text>();
	}

	// ��� ���� �޾ƿ� �׿� �°� �ʱ�ȭ..
	private void SetupChampBattleStatistics(ChampionBattleStatistics battleStatistics)
	{
		int totalBanpickCount = battleStatistics.totalBanCount + battleStatistics.totalPickCount;
		int totalBattleGameCount = battleStatistics.totalPickCount;

		double winRate = 0.0;
		double banpickRate = 0.0;
		if (totalBanpickCount > 0)
		{
			winRate = battleStatistics.totalWinCount / (double)totalBanpickCount * 100.0;
			banpickRate = s_dataTableManager.statisticsDataTable.totalBattleDayCount / (double)totalBanpickCount * 100.0;
		}

		_totalBattleGameCountText.text = StringTable.GetString(battleStatistics.totalWinCount);
		_totalWinCountText.text = StringTable.GetString(totalBattleGameCount);
		_totalLoseCountText.text = StringTable.GetString(totalBattleGameCount - battleStatistics.totalWinCount);
		_winRateText.text = StringTable.GetString(Math.Round(winRate, 2)) + "%";
		_pickCounText.text = StringTable.GetString(battleStatistics.totalPickCount);
		_banCountText.text = StringTable.GetString(battleStatistics.totalBanCount);
		_banpickRateText.text = StringTable.GetString(Math.Round(banpickRate, 2)) + "%";
		_totalAtkDamageText.text = StringTable.GetString(battleStatistics.totalAtkDamageAmount);
		_totalTakeDamageText.text = StringTable.GetString(battleStatistics.totalTakeDamageAount);
		_totalHealText.text = StringTable.GetString(battleStatistics.totalHealAmount);
	}
}
