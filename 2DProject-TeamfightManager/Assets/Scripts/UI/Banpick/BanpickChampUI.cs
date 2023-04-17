using System;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BanpickChampUI : UIBase
{
	public event Action<string> OnButtonClicked;
	public event Action<string> OnButtonHover;

	private ChampionDataTable _champDataTable;
	private BattleStageDataTable _battlestageDataTable;

	private BanpickChampButtonUI _buttonUI;
	private ShowChampAnimUI _champAnimUI;
	private BanpickOutlineUI _outlineUI;
	[SerializeField] private TMP_Text _champNameText;
	private ShowBanEffectUI _banEffectUI;
	private ShowPickEffectUI _pickEffectUI;

	public bool isLockSelect { private get; set; }

	private string _championName;
	public string championName
	{
		get => _championName;
		set
		{
			_championName = value;

			_champNameText.text = _champDataTable.GetChampionData(_championName).nameKR;
			_champAnimUI.SetChampionName(_championName);
		}
	}

	private void Awake()
	{
		_champDataTable = s_dataTableManager.championDataTable;
		_battlestageDataTable = s_dataTableManager.battleStageDataTable;

		_buttonUI = GetComponentInChildren<BanpickChampButtonUI>();
		_champAnimUI = GetComponentInChildren<ShowChampAnimUI>();
		_outlineUI = GetComponentInChildren<BanpickOutlineUI>();
		_banEffectUI = GetComponentInChildren<ShowBanEffectUI>();
		_pickEffectUI = GetComponentInChildren<ShowPickEffectUI>();

		_banEffectUI.gameObject.SetActive(false);
		_pickEffectUI.gameObject.SetActive(false);
		_outlineUI.gameObject.SetActive(false);

		_buttonUI.OnButtonClicked -= OnChampionButtonClick;
		_buttonUI.OnButtonClicked += OnChampionButtonClick;

		_buttonUI.OnStartButtonHover -= OnStartButtonHover;
		_buttonUI.OnStartButtonHover += OnStartButtonHover;

		_buttonUI.OnExitButtonHover -= OnEndButtonHover;
		_buttonUI.OnExitButtonHover += OnEndButtonHover;
	}

	private void OnChampionButtonClick()
	{
		_buttonUI.OnButtonClicked -= OnChampionButtonClick;
		_buttonUI.OnButtonSelect();
		OnButtonClicked?.Invoke(championName);
	}

	private void OnStartButtonHover()
	{
		OnButtonHover?.Invoke(championName);

		if(false == isLockSelect)
		{
			_champAnimUI.SetHoverState(true);
		}

		_outlineUI.SetTeamKind(_battlestageDataTable.curBanpickStageInfo.teamKind);
		_outlineUI.gameObject.SetActive(true);
	}

	private void OnEndButtonHover()
	{
		if(false == isLockSelect)
		{
			_champAnimUI.SetHoverState(false);
		}

		_outlineUI.gameObject.SetActive(false);
	}

	public void ChangeBanpickState(BanpickStageKind state, BattleTeamKind teamKind, int index)
	{
		_champAnimUI.ChangeBanPickData(state, teamKind);

		if(state == BanpickStageKind.Ban)
		{
			_banEffectUI.gameObject.SetActive(true);
		}
		else if( state == BanpickStageKind.Pick)
		{
			_pickEffectUI.SetPickData(teamKind, index);
			_pickEffectUI.gameObject.SetActive(true);
		}
	}
}