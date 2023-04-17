using UnityEngine.UI;

/// <summary>
/// 챔피언 숙련도를 화면에 보여주는 UI..
/// </summary>
public class ChampSkillLevelUI : UIBase
{
    private Text _levelText;
    private Image _champIconImage;

	private void Awake()
	{
		_champIconImage = transform.Find("ChampIconImage").GetComponent<Image>();
		_levelText = transform.Find("LevelBG").GetComponentInChildren<Text>();
	}

	// 파일럿의 챔피언 숙련도를 받아와 UI를 갱신해주는 함수..
	public void SetChampionSkillLevel(ChampionSkillLevelInfo championSkillLevelInfo)
	{
		_levelText.text = StringTable.GetString(championSkillLevelInfo.level);
		_champIconImage.sprite = s_dataTableManager.championDataTable.GetChampionImage(championSkillLevelInfo.champName);
	}
}
