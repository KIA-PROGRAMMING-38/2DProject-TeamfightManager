using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonGroupUIManager : UIBase
{
    private MenuButtonGroupUI[] _menuButtonGroupUIs;
    private MenuButtonGroupUI _curActiveMenuButtonGroupUI = null;

	private void Awake()
	{
		_menuButtonGroupUIs = GetComponentsInChildren<MenuButtonGroupUI>();

		int menuButtonGroupCount = _menuButtonGroupUIs.Length;
		for( int i = 0; i < menuButtonGroupCount; ++i )
		{
			_menuButtonGroupUIs[i].manager = this;
		}
	}

	public void OnClickButton(MenuButtonGroupUI buttonGroup)
    {
		if (null != _curActiveMenuButtonGroupUI)
		{
			_curActiveMenuButtonGroupUI.SetActiveSubGroup(false);
		}

		_curActiveMenuButtonGroupUI = buttonGroup;
		_curActiveMenuButtonGroupUI.SetActiveSubGroup(true);
	}

	public void OnClickBackgroundButton()
	{
		if(null != _curActiveMenuButtonGroupUI)
		{
			_curActiveMenuButtonGroupUI.SetActiveSubGroup(false);
			_curActiveMenuButtonGroupUI = null;
		}
	}

	public void GoToStadiumScene()
	{
		s_gameManager.ChangeScene(SceneNameTable.STADIUM);
	}
}
