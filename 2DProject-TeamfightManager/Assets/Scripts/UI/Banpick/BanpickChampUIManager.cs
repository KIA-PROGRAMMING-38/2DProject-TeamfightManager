using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BanpickChampUIManager : UIBase
{
	private BanpickChampUI[] _allChampUI;
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

	private void SetupChampUI()
	{
		// 모든 밴픽 챔피언 UI를 일단 끈다..
		{
			_allChampUI = GetComponentsInChildren<BanpickChampUI>();
			int loopCount = _allChampUI.Length;
			for( int i = 0; i < loopCount; ++i)
			{
				_allChampUI[i].gameObject.SetActive(false);
			}
		}

		// 챔피언 개수만큼 UI를 생성하며 챔피언 이름도 같이 넘겨준다..
		{
			ChampionDataTable champDataTable = s_dataTableManager.championDataTable;

			int loopCount = champDataTable.GetTotalChampionCount();
			for (int i = 0; i < loopCount; ++i)
			{
				string champName = champDataTable.GetChampionName(i);

				BanpickChampUI ui = _allChampUI[i];
				ui.gameObject.SetActive(true);
				ui.championName = champName;
				_activeChampUIContainer.Add(champName, ui);
			}
		}
	}

	public void OnSelectChampionButtonClicked(string championName)
	{
		_dataTable.OnClickedSelectChampButton(championName);
	}

	private void OnUpdateBackpickStage(string champName, BanpickStageKind stageKind, BattleTeamKind teamKind, int index)
	{
		Debug.Assert( true == _activeChampUIContainer.ContainsKey( champName ) );

		_activeChampUIContainer[champName].ChangeBanpickState( stageKind );
    }
}