using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ShowChampDataUI : UIBase
{
    private ShowChampStatusUI _statusUI;
    [SerializeField] private ShowAtkActionDataUI _skillActionUI;
    [SerializeField] private ShowAtkActionDataUI _ultimateActionUI;

    [SerializeField] private TMP_Text _champNameText;
    [SerializeField] private TMP_Text _champClassTypeText;
	[SerializeField] private Image _champIconImage;

    private ChampionDataTable _champDataTable;
    private AttackActionDataTable _atkActionDataTable;

	private Vector2 _champIconImageStartPos;

	private void Awake()
	{
		_champDataTable = s_dataTableManager.championDataTable;
        _atkActionDataTable = s_dataTableManager.attackActionDataTable;

		_statusUI = GetComponentInChildren<ShowChampStatusUI>();

		_champIconImageStartPos = _champIconImage.rectTransform.localPosition;
	}

    public void ChangeChampionData(string championName)
    {
		// è�Ǿ� �����͸� ������ ���̺��� �����´�..
		ChampionData champData = _champDataTable.GetChampionData(championName);
		ChampionStatus champStatus = _champDataTable.GetChampionStatus(championName);

		// ������ �����ͷ� �ʱ�ȭ �۾�..
		Sprite champIconSprite = _champDataTable.GetChampionImage(championName);
		_champNameText.text = champData.nameKR;
		_champClassTypeText.text = ChampionUtility.CalcClassTypeName(champData.type);
		_champIconImage.sprite = champIconSprite;

		// Champion Icon Image ��ġ ������..
		UIUtility.CalcSpriteCenterPos(_champIconImage.rectTransform, champIconSprite, _champIconImageStartPos);

		_statusUI.status = champStatus;

		Sprite iconSprite = _champDataTable.GetSkillIconImage(championName);
		AttackActionData atkActionData = _atkActionDataTable.GetActionData(champData.skillActionUniqueKey);
		_skillActionUI.ChangeData(iconSprite, champStatus.skillCooltime, false,
			atkActionData.isPassive, atkActionData.description);

		iconSprite = _champDataTable.GetUltimateIconImage(championName);
		atkActionData = _atkActionDataTable.GetActionData(champData.ultimateActionUniqueKey);
		_ultimateActionUI.ChangeData(iconSprite, 0f, true,
			atkActionData.isPassive, atkActionData.description);
	}
}
