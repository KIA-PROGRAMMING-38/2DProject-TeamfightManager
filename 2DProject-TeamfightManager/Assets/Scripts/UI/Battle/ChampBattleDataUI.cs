using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챔피언의 KDA, 피해량, 맞은량, 힐량을 보여주는 UI..
/// </summary>
public class ChampBattleDataUI : MonoBehaviour
{
	private const int KILL_TEXT_INDEX = 1;
	private const int DEATH_TEXT_INDEX = 2;
	private const int ASSIST_TEXT_INDEX = 3;
	private const int TOTAL_ATKDAMAGE_TEXT_INDEX = 4;
	private const int TOTAL_HIT_TEXT_INDEX = 5;
	private const int TOTAL_HEAL_TEXT_INDEX = 6;

	// 챔피언 전투 관련 정보를 화면에 띄워주기 위한 UI..
	private Text _killCountText;
    private Text _deathCountText;
    private Text _assistCountText;
    private Text _totalAtkDamageText;
    private Text _totalHitText;
    private Text _totalHealText;

	[SerializeField] private Image _backgroundImage;

    [SerializeField] private Sprite _redTeamBackgroundSprite;
    [SerializeField] private Sprite _blueTeamBackgroundSprite;

	private void Awake()
	{
		_killCountText = transform.GetChild(KILL_TEXT_INDEX).GetComponent<Text>();
		_deathCountText = transform.GetChild(DEATH_TEXT_INDEX).GetComponent<Text>();
		_assistCountText = transform.GetChild(ASSIST_TEXT_INDEX).GetComponent<Text>();
		_totalAtkDamageText = transform.GetChild(TOTAL_ATKDAMAGE_TEXT_INDEX).GetComponent<Text>();
		_totalHitText = transform.GetChild(TOTAL_HIT_TEXT_INDEX).GetComponent<Text>();
		_totalHealText = transform.GetChild(TOTAL_HEAL_TEXT_INDEX).GetComponent<Text>();
	}

	// 챔피언 전투 정보가 갱신되면 호출되는 콜백 함수..
	public void UpdateData(BattleInfoData data)
    {
		_killCountText.text = StringTable.GetString(data.killCount);
		_deathCountText.text = StringTable.GetString(data.deathCount);
		_assistCountText.text = StringTable.GetString(data.assistCount);
		_totalAtkDamageText.text = StringTable.GetString(data.totalAtkDamage);
		_totalHitText.text = StringTable.GetString(data.totalTakeDamage);
		_totalHealText.text = StringTable.GetString(data.totalHeal);
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
