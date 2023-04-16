using System;
using TMPro;
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

	public BanpickStageKind kind
	{
		set
		{
			switch (value)
			{
				case BanpickStageKind.Ban:

					break;
				case BanpickStageKind.Pick:

					break;
			}
		}
	}

	private void Awake()
	{
		_champDataTable = s_dataTableManager.championDataTable;
		_battlestageDataTable = s_dataTableManager.battleStageDataTable;

		_buttonUI = GetComponentInChildren<BanpickChampButtonUI>();
		_champAnimUI = GetComponentInChildren<ShowChampAnimUI>();
		_outlineUI = GetComponentInChildren<BanpickOutlineUI>();

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
		OnButtonClicked?.Invoke(championName);
	}

	private void OnStartButtonHover()
	{
		OnButtonHover?.Invoke(championName);
		_champAnimUI.ChangeRotateState(true);

		_outlineUI.SetTeamKind(_battlestageDataTable.curBanpickStageInfo.teamKind);
		_outlineUI.gameObject.SetActive(true);
	}

	private void OnEndButtonHover()
	{
		_champAnimUI.ChangeRotateState(false);
		_outlineUI.gameObject.SetActive(false);
	}

	public void ChangeBanpickState(BanpickStageKind state)
	{
		if (BanpickStageKind.Pick == state)
		{

		}
	}
}