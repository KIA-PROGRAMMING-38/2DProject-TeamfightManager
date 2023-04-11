using UnityEngine.UI;

public class ChampSkillLevelUI : UIBase
{
    private Text _levelText;
    private Image _champIconImage;

	private void Awake()
	{
		_champIconImage = transform.Find("ChampIconImage").GetComponent<Image>();
		_levelText = transform.Find("LevelBG").GetComponentInChildren<Text>();
	}

	public void SetChampionSkillLevel(ChampionSkillLevelInfo championSkillLevelInfo)
	{
		_levelText.text = championSkillLevelInfo.level.ToString();
		_champIconImage.sprite = s_dataTableManager.championDataTable.GetChampionImage(championSkillLevelInfo.champName);
	}
}
