using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsMenuButtonGroupUI : MenuButtonGroupUI
{
	private enum SubButtonKind
	{
		TeamStatistics,
		ChampionStatistics,
		ChampionInfomation
	}

	public override void OnClickSubGroupButton(int buttonKey)
	{
		SubButtonKind subButtonKind = (SubButtonKind)buttonKey;

		switch (subButtonKind)
		{
			case SubButtonKind.TeamStatistics:
				break;
			case SubButtonKind.ChampionStatistics:
				break;
			case SubButtonKind.ChampionInfomation:
				break;
		}
	}
}
