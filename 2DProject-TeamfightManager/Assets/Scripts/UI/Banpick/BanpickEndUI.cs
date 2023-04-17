using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanpickEndUI : UIBase
{
    private GameObject _child;

    // Start is called before the first frame update
    void Awake()
    {
		_child = transform.GetChild(0).gameObject;
        _child.SetActive(false);
	}

    public void OnEndBanpick()
    {
		_child.SetActive(true);
	}

    public void OnStartBattleButtonClick()
    {
        s_dataTableManager.battleStageDataTable.StartBattle();
    }
}
