using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager
    {
        set
        {
			UIBase.s_uiManager = this;
			UIBase.s_dataTableManager = value.dataTableManager;
		}
    }
}
