using UnityEngine.UI;

/// <summary>
/// è�Ǿ� ���õ��� ȭ�鿡 �����ִ� UI..
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

	// ���Ϸ��� è�Ǿ� ���õ��� �޾ƿ� UI�� �������ִ� �Լ�..
	public void SetChampionSkillLevel(ChampionSkillLevelInfo championSkillLevelInfo)
	{
		_levelText.text = StringTable.GetString(championSkillLevelInfo.level);
		_champIconImage.sprite = s_dataTableManager.championDataTable.GetChampionImage(championSkillLevelInfo.champName);
	}
}
