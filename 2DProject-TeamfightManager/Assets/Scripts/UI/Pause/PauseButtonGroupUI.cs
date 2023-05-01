using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonGroupUI : UIBase
{
    private GameObject _uiParentObject;
	private GameObject _backgroundImageObject;

	private void Awake()
	{
		_uiParentObject = transform.GetChild(0).gameObject;
		_uiParentObject.SetActive(false);

		_backgroundImageObject = transform.parent.GetChild(0).gameObject;
		_backgroundImageObject.SetActive(false);
	}

	private void Update()
	{
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
			_uiParentObject.SetActive(true);
			_backgroundImageObject.SetActive(true);
		}
	}

	public void ResumeGame()
    {
        Time.timeScale = 1f;
		_uiParentObject.SetActive(false);
		_backgroundImageObject.SetActive(false);
	}

	public void GoToTitleScene()
    {
		s_gameManager.ChangeScene(SceneNameTable.TITLE);
    }
}
