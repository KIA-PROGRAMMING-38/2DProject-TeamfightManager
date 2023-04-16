using UnityEngine;

public class ShowBanChampUIManager : UIBase
{
    ShowBanChampUI[][] _allBanChampUI;

	private void Awake()
	{
		SetupBanUI();

		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanData;
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate += UpdateBanData;
	}

	private void OnDisable()
	{
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanData;
	}

	private void SetupBanUI()
	{
		ShowBanChampUI[] findRedTeamChampUI = transform.Find("RedTeam").GetComponentsInChildren<ShowBanChampUI>();
		ShowBanChampUI[] findBlueTeamChampUI = transform.Find("BlueTeam").GetComponentsInChildren<ShowBanChampUI>();

		int battleTeamCount = (int)BattleTeamKind.End;
		_allBanChampUI = new ShowBanChampUI[battleTeamCount][];

		int teamChampCount = findRedTeamChampUI.Length;
		for (int i = 0; i < teamChampCount; ++i)
		{
			_allBanChampUI[(int)BattleTeamKind.RedTeam] = new ShowBanChampUI[teamChampCount];
			_allBanChampUI[(int)BattleTeamKind.BlueTeam] = new ShowBanChampUI[teamChampCount];

			_allBanChampUI[(int)BattleTeamKind.RedTeam][i] = findRedTeamChampUI[i];
			_allBanChampUI[(int)BattleTeamKind.BlueTeam][i] = findBlueTeamChampUI[i];
		}
	}

	private void UpdateBanData(string champName, BanpickStageKind banpickKind, BattleTeamKind teamKind, int index)
	{
		if (banpickKind == BanpickStageKind.Ban)
			_allBanChampUI[(int)teamKind][index].OnBan(champName);
	}
}
