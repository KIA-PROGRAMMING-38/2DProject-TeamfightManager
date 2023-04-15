using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager = null;
    public GameManager gameManager
    {
        get => _gameManager;
		set
        {
			UIBase.s_uiManager = this;
			UIBase.s_dataTableManager = value.dataTableManager;
		}
    }

    public Dictionary<string, UIBase> _uiContainer = new Dictionary<string, UIBase>();

    public void AddUI(string name, UIBase uiInstance)
    {
        _uiContainer.Add(name, uiInstance);
	}

    public void RemoveUI(string name)
    {
        _uiContainer.Remove(name);
	}

    public void Clear()
    {
        _uiContainer.Clear();
	}
}
