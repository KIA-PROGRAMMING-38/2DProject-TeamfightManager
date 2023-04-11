using UnityEngine;
using UnityEngine.UI;

public class PilotDataUI : UIBase
{
	[SerializeField] private Image _backgroundImage;

	[SerializeField] private Sprite _redTeamBackgroundSprite;
	[SerializeField] private Sprite _blueTeamBackgroundSprite;

	[SerializeField] private Text _pilotNameText;
	[SerializeField] private Text _pilotAtkStatText;
	[SerializeField] private Text _pilotDefenceStatText;
	[SerializeField] private Image _champIconImage;

	private ChampSkillLevelUIManager _champSkillLevelUIManager;
	
	// 데이터를 출력할 파일럿을 받아 초기화한다..
	public Pilot pilot
	{
		set
		{
			_pilotNameText.text = value.data.name;
			_pilotAtkStatText.text = value.data.atkStat.ToString();
			_pilotDefenceStatText.text = value.data.defStat.ToString();

			Sprite champSprite = s_dataTableManager.championDataTable.GetChampionImage(value.battleComponent.controlChampion.data.name);
			SetupChampionIconSprite(champSprite);

			_champSkillLevelUIManager.SetChampSkillLevelInfo(value.data.champSkillLevelContainer);
		}
	}

	private void Awake()
	{
		_champSkillLevelUIManager = GetComponentInChildren<ChampSkillLevelUIManager>();
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

	private void SetupChampionIconSprite(Sprite champSprite)
	{
		// Champion Icon Image 바꿔주기..
		_champIconImage.sprite = champSprite;

		// Champion Icon Image 가 가운데 올 수 있도록 위치 조정..
		float imageWidth = _champIconImage.rectTransform.sizeDelta.x;
		Vector2 localPos = _champIconImage.rectTransform.localPosition;

		float ratio = champSprite.pivot.x / champSprite.texture.width - 0.5f;
		localPos.x = localPos.x - ratio * imageWidth;

		_champIconImage.rectTransform.localPosition = localPos;
	}
}