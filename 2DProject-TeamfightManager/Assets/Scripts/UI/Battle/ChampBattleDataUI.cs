using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// è�Ǿ��� KDA, ���ط�, ������, ������ �����ִ� UI..
/// </summary>
public class ChampBattleDataUI : MonoBehaviour
{
	private const int KILL_TEXT_INDEX = 1;
	private const int DEATH_TEXT_INDEX = 2;
	private const int ASSIST_TEXT_INDEX = 3;
	private const int TOTAL_ATKDAMAGE_TEXT_INDEX = 4;
	private const int TOTAL_HIT_TEXT_INDEX = 5;
	private const int TOTAL_HEAL_TEXT_INDEX = 6;

	// è�Ǿ� ���� ���� ������ ȭ�鿡 ����ֱ� ���� UI..
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

	// è�Ǿ� ���� ������ ���ŵǸ� ȣ��Ǵ� �ݹ� �Լ�..
	public void UpdateData(BattleInfoData data)
    {
		_killCountText.text = StringTable.GetString(data.killCount);
		_deathCountText.text = StringTable.GetString(data.deathCount);
		_assistCountText.text = StringTable.GetString(data.assistCount);
		_totalAtkDamageText.text = StringTable.GetString(data.totalAtkDamage);
		_totalHitText.text = StringTable.GetString(data.totalTakeDamage);
		_totalHealText.text = StringTable.GetString(data.totalHeal);
	}

	// UI�� ����� è�Ǿ��� ���� ���� ������ �ٲ��ֱ� ���� �Լ�..
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
