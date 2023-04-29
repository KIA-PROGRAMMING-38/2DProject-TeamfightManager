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
#endif

        Application.Quit();
    }
}
