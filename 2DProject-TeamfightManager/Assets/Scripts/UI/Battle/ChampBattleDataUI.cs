using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// è�Ǿ��� KDA, ���ط�, ������, ������ �����ִ� UI..
/// </summary>
public class ChampBattleDataUI : MonoBehaviour
{
	// è�Ǿ� ���� ���� ������ ȭ�鿡 ����ֱ� ���� UI..
    [SerializeField] private Text _killCountText;
    [SerializeField] private Text _deathCountText;
    [SerializeField] private Text _assistCountText;
    [SerializeField] private Text _totalAtkDamageText;
    [SerializeField] private Text _totalHitText;
    [SerializeField] private Text _totalHillText;

	[SerializeField] private Image _backgroundImage;

    [SerializeField] private Sprite _redTeamBackgroundSprite;
    [SerializeField] private Sprite _blueTeamBackgroundSprite;

	// è�Ǿ� ���� ������ ���ŵǸ� ȣ��Ǵ� �ݹ� �Լ�..
	public void UpdateData(BattleInfoData data)
    {
		_killCountText.text = StringTable.GetString(data.killCount);
		_deathCountText.text = StringTable.GetString(data.deathCount);
		_assistCountText.text = StringTable.GetString(data.assistCount);
		_totalAtkDamageText.text = StringTable.GetString(data.totalDamage);
		_totalHitText.text = StringTable.GetString(data.totalHit);
		_totalHillText.text = StringTable.GetString(data.totalHill);
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
