using UnityEngine;
using UnityEngine.UI;

public class PilotDataUI : MonoBehaviour
{
	[SerializeField] private Image _backgroundImage;

	[SerializeField] private Sprite _redTeamBackgroundSprite;
	[SerializeField] private Sprite _blueTeamBackgroundSprite;

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
}