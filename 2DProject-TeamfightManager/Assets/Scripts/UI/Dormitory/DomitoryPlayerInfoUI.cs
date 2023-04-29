using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomitoryPlayerInfoUI : UIBase
{
    private Image _teamLogoImage;
    private TMP_Text _teamNameText;

	private void Awake()
	{
		Transform teamInfoTransform = transform.Find("MyTeamInfo");
		_teamLogoImage = teamInfoTransform.Find("LogoImage").GetComponent<Image>();
		_teamNameText = teamInfoTransform.Find("NameText").GetComponent<TMP_Text>();
	}

	private void OnEnable()
	{
		string playerTeamName = s_gameManager.gameGlobalData.playerTeamName;
		_teamLogoImage.sprite = s_dataTableManager.teamDataTable.GetLogoSprite(playerTeamName);
		_teamNameText.text = playerTeamName;
	}
}
