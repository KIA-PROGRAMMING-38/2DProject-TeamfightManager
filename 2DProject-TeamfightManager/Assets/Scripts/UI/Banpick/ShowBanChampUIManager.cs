using UnityEngine;

public class ShowBanChampUIManager : UIBase
{
	[SerializeField] private ShowBanChampUI _banChampUIPrefab;
    private ShowBanChampUI[][] _allBanChampUI;

	private void Awake()
	{
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanData;
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate += UpdateBanData;
	}

	private void OnDisable()
	{
		s_dataTableManager.battleStageDataTable.OnBanpickUpdate -= UpdateBanData;
	}

	private void Start()
	{
		SetupBanUI();
	}

	private void SetupBanUI()
	{
		Transform redTeamParent = transform.Find("RedTeam");
		Transform blueTeamParent = transform.Find("BlueTeam");

		int battleTeamCount = (int)BattleTeamKind.End;
		_allBanChampUI = new ShowBanChampUI[battleTeamCount][];

		int banChampCount = s_dataTableManager.battleStageDataTable.totalBanChampCount;
		_allBanChampUI[(int)BattleTeamKind.RedTeam] = new ShowBanChampUI[banChampCount];
		_allBanChampUI[(int)BattleTeamKind.BlueTeam] = new ShowBanChampUI[banChampCount];

		for (int i = 0; i < banChampCount; ++i)
		{
			_allBanChampUI[(int)BattleTeamKind.RedTeam][i] = Instantiate(_banChampUIPrefab, redTeamParent);
			_allBanChampUI[(int)BattleTeamKind.BlueTeam][i] = Instantiate(_banChampUIPrefab, blueTeamParent);
		}
	}

	private void UpdateBanData(string champName, BanpickStageKind banpickKind, BattleTeamKind teamKind, int index)
	{
		if (banpickKind == BanpickStageKind.Ban)
			_allBanChampUI[(int)teamKind][index].OnBan(champName);
	}
}
