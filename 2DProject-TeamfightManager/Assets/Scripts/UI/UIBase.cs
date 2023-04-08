using UnityEngine;

public class UIBase : MonoBehaviour
{
    public static UIManager s_uiManager { protected get; set; }
    protected DataTableManager dataTableManager { get; private set; }
    
    protected void Awake()
    {
        DataTableManager tmp;
		s_uiManager.GetDataTableManager(out tmp);

#if UNITY_EDITOR
        Debug.Assert(null != tmp, "UIBase : Data Table Manager is null");
#endif

        dataTableManager = tmp;
    }
}
