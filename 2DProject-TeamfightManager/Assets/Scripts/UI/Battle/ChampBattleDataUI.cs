using UnityEngine;
using UnityEngine.UI;

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
        _killCountText.text = data.killCount.ToString();
		_deathCountText.text = data.deathCount.ToString();
		_assistCountText.text = data.assistCount.ToString();
		_totalAtkDamageText.text = data.totalDamage.ToString();
		_totalHitText.text = data.totalHit.ToString();
		_totalHillText.text = data.totalHill.ToString();
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
