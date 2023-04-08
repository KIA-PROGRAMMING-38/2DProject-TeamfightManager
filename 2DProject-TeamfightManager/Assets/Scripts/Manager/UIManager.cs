using UnityEngine;

public class UIManager : MonoBehaviour
{
    private DataTableManager _dataTableManager;

    public GameManager gameManager
    {
        set
        {
            _dataTableManager = value.dataTableManager;
        }
    }

	private void Awake()
	{
        UIBase.s_uiManager = this;
	}

    public bool GetDataTableManager(out DataTableManager getDataTableManager)
    {
        if (null == _dataTableManager)
        {
			getDataTableManager = null;

			return false;
		}

        getDataTableManager = _dataTableManager;

        return true;
    }
}
