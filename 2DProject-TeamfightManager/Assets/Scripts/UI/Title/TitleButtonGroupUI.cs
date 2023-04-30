using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleButtonGroupUI : UIBase
{
    public void OnClickNewGameButton()
    {
        s_gameManager.ChangeScene(SceneNameTable.DORMITORY);
	}

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}
