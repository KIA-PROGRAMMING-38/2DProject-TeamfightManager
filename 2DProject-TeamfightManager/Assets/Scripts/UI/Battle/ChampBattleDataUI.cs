using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챔피언의 KDA, 피해량, 맞은량, 힐량을 보여주는 UI..
/// </summary>
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
		_killCountText.text = StringTable.GetString(data.killCount);
		_deathCountText.text = StringTable.GetString(data.deathCount);
		_assistCountText.text = StringTable.GetString(data.assistCount);
		_totalAtkDamageText.text = StringTable.GetString(data.totalDamage);
		_totalHitText.text = StringTable.GetString(data.totalHit);
		_totalHillText.text = StringTable.GetString(data.totalHill);
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
