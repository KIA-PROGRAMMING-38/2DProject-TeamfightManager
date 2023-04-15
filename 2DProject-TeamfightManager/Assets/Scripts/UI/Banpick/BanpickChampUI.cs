using System;
using UnityEngine.UI;

public class BanpickChampUI : UIBase
{
	public event Action<string> OnButtonClicked;

	private string _championName;
	public string championName
	{
		get => _championName;
		set
		{
			_championName = value;

			GetComponentInChildren<BanpickChampButtonUI>().SetChampionName(_championName);
		}
	}

	private void Awake()
	{
		BanpickChampButtonUI button = GetComponentInChildren<BanpickChampButtonUI>();

		button.OnButtonClicked -= OnChampButtonClicked;
		button.OnButtonClicked += OnChampButtonClicked;
	}

	private void OnChampButtonClicked()
	{
		OnButtonClicked?.Invoke(championName);
	}
}