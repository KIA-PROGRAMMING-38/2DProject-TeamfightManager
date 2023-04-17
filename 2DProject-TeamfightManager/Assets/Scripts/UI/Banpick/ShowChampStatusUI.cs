using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowChampStatusUI : UIBase
{
	[SerializeField] private TMP_Text _atkStatText;
	[SerializeField] private TMP_Text _defStatText;
	[SerializeField] private TMP_Text _atkSpeedText;
	[SerializeField] private TMP_Text _hpText;
	[SerializeField] private TMP_Text _rangeText;
	[SerializeField] private TMP_Text _moveSpeedText;

	public ChampionStatus status
	{
		set
		{
			_atkStatText.text = StringTable.GetString(value.atkStat);
			_defStatText.text = StringTable.GetString(value.defence);
			_atkSpeedText.text = StringTable.GetString(value.atkSpeed);
			_hpText.text = StringTable.GetString(value.hp);
			_rangeText.text = StringTable.GetString(value.range);
			_moveSpeedText.text = StringTable.GetString(value.moveSpeed);
		}
	}
}
