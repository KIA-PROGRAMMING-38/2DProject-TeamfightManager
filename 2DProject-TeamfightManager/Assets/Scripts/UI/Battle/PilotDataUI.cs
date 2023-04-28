using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
	private UIMove _champImageMove;

	[SerializeField] private UIMove _pilotImageMove;
	private Image _pilotHairImage;

	private ChampSkillLevelUIManager _champSkillLevelUIManager;

	// 데이터를 출력할 파일럿을 받아 초기화한다..
	private Pilot _pilot;
	public Pilot pilot
	{
		private get => _pilot;
		set
		{
			_pilot = value;

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

	// Champion Icon Image를 외부에서 받아와 화면에 보여준다..
	public void ChangePilotIconImage(Sprite sprite, string championName)
	{
		// 파일럿 능력치 갱신(숙련도 있는 챔피언을 픽했을 수도 있기 때문에)..
		if (0 < _pilot.battleComponent.plusStat)
		{
			_pilotAtkStatText.text = StringTable.GetString(_pilot.battleComponent.atkStat);
			_pilotDefenceStatText.text = StringTable.GetString(_pilot.battleComponent.defStat);

			int loopCount = _pilot.data.champSkillLevelContainer.Count;
			for( int i = 0; i < loopCount; ++i)
			{
				if (_pilot.data.champSkillLevelContainer[i].champName == championName)
				{
					_champSkillLevelUIManager.ShowSelectImage(i);

					break;
				}
			}
		}

		// Champion Icon Image 바꿔주기..
		_champIconImage.sprite = sprite;

        // Champion Icon Image 가 가운데 올 수 있도록 위치 조정..
        UIUtility.CalcSpriteCenterPos(_champIconImage.rectTransform, sprite, _champIconImage.rectTransform.localPosition);

        _champImageMove.StartMove(false);
		_pilotImageMove.StartMove(false);
    }
}