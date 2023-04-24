using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleButtonGroupUI : UIBase
{
    public void OnClickNewGameButton()
    {
        s_gameManager.ChangeScene(SceneNameTable.DORMITORY);
	}
}
