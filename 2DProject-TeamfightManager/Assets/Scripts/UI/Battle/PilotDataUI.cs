using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 파일럿의 정보 및 파일럿이 현재 픽한 챔피언의 이미지를 화면에 보여주는 UI..
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

	private ChampSkillLevelUIManager _champSkillLevelUIManager;
	
	// 데이터를 출력할 파일럿을 받아 초기화한다..
	public Pilot pilot
	{
		set
		{
			_pilotNameText.text = value.data.name;
			_pilotAtkStatText.text = StringTable.GetString(value.data.atkStat);
			_pilotDefenceStatText.text = StringTable.GetString(value.data.defStat);

			//Sprite champSprite = s_dataTableManager.championDataTable.GetChampionImage(value.battleComponent.controlChampion.data.name);
			//SetupChampionIconSprite(champSprite);

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

	// Champion Icon Image를 외부에서 받아와 화면에 보여준다..
	public void ChangePilotIconImage(Sprite sprite)
	{
		// Champion Icon Image 바꿔주기..
		_champIconImage.sprite = sprite;

		// Champion Icon Image 가 가운데 올 수 있도록 위치 조정..
		UIUtility.CalcSpriteCenterPos(_champIconImage.rectTransform, sprite, _champIconImage.rectTransform.localPosition);
	}
}