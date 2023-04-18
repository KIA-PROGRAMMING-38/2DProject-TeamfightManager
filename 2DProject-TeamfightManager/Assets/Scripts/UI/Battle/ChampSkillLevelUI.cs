using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// è�Ǿ� ���õ��� ȭ�鿡 �����ִ� UI..
/// </summary>
public class ChampSkillLevelUI : UIBase
{
    [SerializeField] private Text _levelText;
    [SerializeField] private Image _champIconImage;

	// ���Ϸ��� è�Ǿ� ���õ��� �޾ƿ� UI�� �������ִ� �Լ�..
	public void SetChampionSkillLevel(ChampionSkillLevelInfo championSkillLevelInfo)
	{
		_levelText.text = StringTable.GetString(championSkillLevelInfo.level);
		_champIconImage.sprite = s_dataTableManager.championDataTable.GetChampionImage(championSkillLevelInfo.champName);

		UIUtility.CalcSpriteCenterPos(_champIconImage.rectTransform, _champIconImage.sprite, _champIconImage.rectTransform.localPosition);
	}
}
