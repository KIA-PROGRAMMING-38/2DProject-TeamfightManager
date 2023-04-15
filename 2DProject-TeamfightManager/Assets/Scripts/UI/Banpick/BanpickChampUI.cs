using System;
using UnityEngine;
using UnityEngine.UI;

public class BanpickChampUI : UIBase
{
	public event Action<string> OnButtonClicked;

	private BanpickChampButtonUI _buttonUI;

	private string _championName;
	public string championName
	{
		get => _championName;
		set
		{
			_championName = value;

			_buttonUI.SetChampionName( _championName );
		}
	}

	public BanpickStageKind kind
	{
		set
		{
            switch ( value )
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
		_buttonUI = GetComponentInChildren<BanpickChampButtonUI>();

		_buttonUI.OnButtonClicked -= OnChampButtonClicked;
		_buttonUI.OnButtonClicked += OnChampButtonClicked;
	}

	private void OnChampButtonClicked()
	{
		OnButtonClicked?.Invoke(championName);
	}

	public void ChangeBanpickState(BanpickStageKind state )
	{
		_buttonUI.ChangeBanpickState( state );
	}
}