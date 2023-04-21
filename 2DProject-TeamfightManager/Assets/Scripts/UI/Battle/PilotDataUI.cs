using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���Ϸ��� ���� �� ���Ϸ��� ���� ���� è�Ǿ��� �̹����� ȭ�鿡 �����ִ� UI..
/// </summary>
public class PilotDataUI : UIBase
{
	[SerializeField] private Image _backgroundImage;

	[SerializeField] private Sprite _redTeamBackgroundSprite;
	[SerializeField] private Sprite _blueTeamBackgroundSprite;

	[SerializeField] private Text _pilotNameText;
	[SerializeField] private Text _pilotAtkStatText;
	[SerializeField] private Text _pilotDefenceStatText;
	[SerializeField] private Image _champIconImage;
	private UIMove _champImageMove;

	[SerializeField] private UIMove _pilotImageMove;
	private Image _pilotHairImage;

	private ChampSkillLevelUIManager _champSkillLevelUIManager;
	
	// �����͸� ����� ���Ϸ��� �޾� �ʱ�ȭ�Ѵ�..
	public Pilot pilot
	{
		set
		{
			_pilotNameText.text = value.data.name;
			_pilotAtkStatText.text = StringTable.GetString(value.data.atkStat);
			_pilotDefenceStatText.text = StringTable.GetString(value.data.defStat);

			Sprite pilotHairSprite = s_dataTableManager.pilotDataTable.GetHairSprite(value.data.hairNumber);
			_pilotHairImage.sprite = pilotHairSprite;

			_champSkillLevelUIManager.SetChampSkillLevelInfo(value.data.champSkillLevelContainer);
		}
	}

	private void Awake()
	{
		_champSkillLevelUIManager = GetComponentInChildren<ChampSkillLevelUIManager>();

		_champImageMove = _champIconImage.transform.parent.GetComponent<UIMove>();
		_pilotHairImage = _pilotImageMove.transform.GetChild(0).GetComponent<Image>();
    }

	public void SetBackgroundImage(BattleTeamKind teamKind)
	{
		switch (teamKind)
		{
			case BattleTeamKind.RedTeam:
				_backgroundImage.sprite = _redTeamBackgroundSprite;
				break;
			case BattleTeamKind.BlueTeam:
				_backgroundImage.sprite = _blueTeamBackgroundSprite;
				break;
		}
	}

	// Champion Icon Image�� �ܺο��� �޾ƿ� ȭ�鿡 �����ش�..
	public void ChangePilotIconImage(Sprite sprite)
	{
		// Champion Icon Image �ٲ��ֱ�..
		_champIconImage.sprite = sprite;

        // Champion Icon Image �� ��� �� �� �ֵ��� ��ġ ����..
        UIUtility.CalcSpriteCenterPos(_champIconImage.rectTransform, sprite, _champIconImage.rectTransform.localPosition);

        _champImageMove.StartMove(false);
		_pilotImageMove.StartMove(false);
    }
}