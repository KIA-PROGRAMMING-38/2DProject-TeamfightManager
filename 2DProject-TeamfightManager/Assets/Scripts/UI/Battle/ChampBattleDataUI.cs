using UnityEngine;
using UnityEngine.UI;

public class ChampBattleDataUI : MonoBehaviour
{
    [SerializeField] private Text _killCountText;
    [SerializeField] private Text _deathCountText;
    [SerializeField] private Text _assistCountText;
    [SerializeField] private Text _totalAtkDamageText;
    [SerializeField] private Text _totalHitText;
    [SerializeField] private Text _totalHillText;

	[SerializeField] private Image _backgroundImage;

    [SerializeField] private Sprite _redTeamBackgroundSprite;
    [SerializeField] private Sprite _blueTeamBackgroundSprite;

	public void UpdateData(BattleInfoData data)
    {
        _killCountText.text = data.killCount.ToString();
		_deathCountText.text = data.deathCount.ToString();
		_assistCountText.text = data.assistCount.ToString();
		_totalAtkDamageText.text = data.totalDamage.ToString();
		_totalHitText.text = data.totalHit.ToString();
		_totalHillText.text = data.totalHill.ToString();
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
}
