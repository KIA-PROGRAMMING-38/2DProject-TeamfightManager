using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenuButtonGroupUI : MenuButtonGroupUI
{
	private enum SubButtonKind
	{
		GoTitle,
		SaveGame,
		EnviormentSetting
	}

	public override void OnClickSubGroupButton(int buttonKey)
	{
		SubButtonKind subButtonKind = (SubButtonKind)buttonKey;
		switch (subButtonKind)
		{
			case SubButtonKind.GoTitle:
				s_gameManager.ChangeScene(SceneNameTable.TITLE);

				break;
			case SubButtonKind.SaveGame:

				break;
			case SubButtonKind.EnviormentSetting:

				break;
		}
	}
}
