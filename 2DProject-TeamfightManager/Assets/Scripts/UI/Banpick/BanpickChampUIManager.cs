using System;
using System.Collections.Generic;
using UnityEngine;

public class BanpickChampUIManager : UIBase
{
	public event Action<string> OnSelectChampionButton;

	public BanpickMainUI banpickMainUI { private get; set; }

	[SerializeField] private BanpickChampUI _banpickChampUIPrefab;
	private Dictionary<string, BanpickChampUI> _activeChampUIContainer = new Dictionary<string, BanpickChampUI>();
	private BattleStageDataTable _dataTable;

	private void Awake()
	{
		// 사용할 데이터 테이블 가져와서 이벤트 등록..
		_dataTable = s_dataTableManager.battleStageDataTable;

		_dataTable.OnBanpickUpdate -= OnUpdateBackpickStage;
		_dataTable.OnBanpickUpdate += OnUpdateBackpickStage;

		SetupChampUI();
	}

	private void Start()
	{

	}

	private void SetupChampUI()
	{
		// 챔피언 개수만큼 UI를 생성하며 챔피언 이름도 같이 넘겨준다..
		{
			ChampionDataTable champDataTable = s_dataTableManager.championDataTable;

			int loopCount = champDataTable.GetTotalChampionCount();
			for (int i = 0; i < loopCount; ++i)
			{
				string champName = champDataTable.GetChampionName(i);

				BanpickChampUI ui = Instantiate(_banpickChampUIPrefab, transform.GetChild(i / 10));
				ui.gameObject.SetActive(true);
				ui.championName = champName;

				// 현재 UI의 이벤트 함수 구독..
				ui.OnButtonClicked -= OnSelectChampionButtonClicked;
				ui.OnButtonClicked += OnSelectChampionButtonClicked;

				ui.OnButtonHover -= ChangeShowChampionData;
				ui.OnButtonHover += ChangeShowChampionData;

				_activeChampUIContainer.Add(champName, ui);
			}
		}
	}

	public void OnSelectChampionButtonClicked(string championName)
	{
		OnSelectChampionButton?.Invoke(championName);

		_activeChampUIContainer[championName].isLockSelect = true;
	}

	private void OnUpdateBackpickStage(string champName, BanpickStageKind stageKind, BattleTeamKind teamKind, int index)
	{
		Debug.Assert( true == _activeChampUIContainer.ContainsKey( champName ) );

		_activeChampUIContainer[champName].ChangeBanpickState( stageKind, teamKind, index );
    }

	public void ChangeShowChampionData(string championName)
	{
		banpickMainUI?.ChangeShowChampionData(championName);
	}
}