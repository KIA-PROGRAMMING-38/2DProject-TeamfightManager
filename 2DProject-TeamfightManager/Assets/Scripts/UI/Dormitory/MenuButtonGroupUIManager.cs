using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonGroupUIManager : UIBase
{
    private MenuButtonGroupUI[] _menuButtonGroupUIs;
    private MenuButtonGroupUI _curActiveMenuButtonGroupUI = null;
	[SerializeField] private DomitorySubMenuGroup _subMenuGroup;

	private void Awake()
	{
		_menuButtonGroupUIs = GetComponentsInChildren<MenuButtonGroupUI>();

		int menuButtonGroupCount = _menuButtonGroupUIs.Length;
		for( int i = 0; i < menuButtonGroupCount; ++i )
		{
			_menuButtonGroupUIs[i].manager = this;
		}
	}

	private void Update()
	{
		if (GameManager.isAutoPlaying)
			GoToStadiumScene();
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

	public void ShowSubMenu(DomitorySubMenuGroup.SubMenuKind subMenuKind)
	{
		_subMenuGroup.OnCloseSubMenu -= OnCloseSubMenu;
		_subMenuGroup.OnCloseSubMenu += OnCloseSubMenu;

		_subMenuGroup.ShowSubMenu(subMenuKind);
	}

	private void OnCloseSubMenu()
	{

	}
}
