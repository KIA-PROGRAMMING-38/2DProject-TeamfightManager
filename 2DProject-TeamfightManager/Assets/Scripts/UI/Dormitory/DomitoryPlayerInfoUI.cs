using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomitoryPlayerInfoUI : UIBase
{
    private Image _teamLogoImage;
    private TMP_Text _teamNameText;
	private TMP_Text _battleResultText;

	private void Awake()
	{
		Transform teamInfoTransform = transform.Find("MyTeamInfo");
		_teamLogoImage = teamInfoTransform.Find("LogoImage").GetComponent<Image>();
		_teamNameText = teamInfoTransform.Find("NameText").GetComponent<TMP_Text>();
		_battleResultText = teamInfoTransform.Find("LeagueBattleData").GetComponent<TMP_Text>();
	}

	private void OnEnable()
	{
		string playerTeamName = s_gameManager.gameGlobalData.playerTeamName;
		_teamLogoImage.sprite = s_dataTableManager.teamDataTable.GetLogoSprite(playerTeamName);
		_teamNameText.text = playerTeamName;

		int totalWinCount = s_dataTableManager.statisticsDataTable.GetTeamBattleStatistics(playerTeamName).totalWinCount;
		int totalLoseCount = s_dataTableManager.statisticsDataTable.GetTeamBattleStatistics(playerTeamName).totalLoseCount;
		int totalDrawCount = s_dataTableManager.statisticsDataTable.GetTeamBattleStatistics(playerTeamName).totalDrawCount;

		int score = totalWinCount * 3 + totalDrawCount;

		_battleResultText.text = StringTable.GetString(totalWinCount) + "½Â " + StringTable.GetString(totalLoseCount) + "ÆÐ " + StringTable.GetString(totalDrawCount) + "¹« " +
			StringTable.GetString(score) + "½ÂÁ¡";
	}
}
