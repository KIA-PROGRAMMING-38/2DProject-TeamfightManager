using UnityEngine;
using UnityEngine.UI;

public class ChampBattleDataUI : MonoBehaviour
{
	// 챔피언 전투 관련 정보를 화면에 띄워주기 위한 UI..
    [SerializeField] private Text _killCountText;
    [SerializeField] private Text _deathCountText;
    [SerializeField] private Text _assistCountText;
    [SerializeField] private Text _totalAtkDamageText;
    [SerializeField] private Text _totalHitText;
    [SerializeField] private Text _totalHillText;

	[SerializeField] private Image _backgroundImage;

    [SerializeField] private Sprite _redTeamBackgroundSprite;
    [SerializeField] private Sprite _blueTeamBackgroundSprite;

	// 챔피언 전투 정보가 갱신되면 호출되는 콜백 함수..
	public void UpdateData(BattleInfoData data)
    {
        _killCountText.text = data.killCount.ToString();
		_deathCountText.text = data.deathCount.ToString();
		_assistCountText.text = data.assistCount.ToString();
		_totalAtkDamageText.text = data.totalDamage.ToString();
		_totalHitText.text = data.totalHit.ToString();
		_totalHillText.text = data.totalHill.ToString();
	}

	// UI가 출력할 챔피언의 팀에 따라 배경색을 바꿔주기 위한 함수..
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
