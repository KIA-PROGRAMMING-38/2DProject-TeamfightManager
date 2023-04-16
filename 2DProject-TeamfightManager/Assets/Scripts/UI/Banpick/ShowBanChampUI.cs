using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowBanChampUI : UIBase
{
	[SerializeField] private GameObject _onBanActiveObjectParent;
    [SerializeField] private Image _champIconImage;
    [SerializeField] private TMP_Text _champNameText;

	private ChampionDataTable _champDataTable;
	private BattleStageDataTable _battleStageDataTable;

	private BanpickOutlineUI _outlineUI;

	private void Awake()
	{
		_champDataTable = s_dataTableManager.championDataTable;
		_battleStageDataTable = s_dataTableManager.battleStageDataTable;

		_onBanActiveObjectParent.SetActive(false);

		_outlineUI = GetComponentInChildren<BanpickOutlineUI>();
		_outlineUI.gameObject.SetActive(false);
	}

	public void OnBan(string champName)
    {
		_onBanActiveObjectParent.SetActive(true);

		_champIconImage.sprite = _champDataTable.GetChampionImage(champName);
		_champNameText.text = _champDataTable.GetChampionData(champName).nameKR;
    }

	public void SetOutlineUIActive(bool active)
	{
		_outlineUI.gameObject.SetActive(active);
		if(true == active)
		{
			_outlineUI.SetTeamKind(_battleStageDataTable.curBanpickStageInfo.teamKind);
		}
	}
}
