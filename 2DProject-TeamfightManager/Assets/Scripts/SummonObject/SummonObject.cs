using System;
using UnityEngine;

public class SummonObject : MonoBehaviour
{
	public event Action<SummonObject, Champion[], int> OnExecuteImpact;
	public event Action<SummonObject> OnRelease;

	public SummonObjectManager summonObjectManager { protected get; set; }
	public EffectManager effectManager { protected get; set; }
	public ChampionManager championManager { protected get; set; }
	public DataTableManager dataTableManager { protected get; set; }

	public string summonObjectName { get => _summonObjectName; }
	[SerializeField] private string _summonObjectName;
	protected Champion[] _targetArray;
	protected Func<Vector3, Champion[], int> _targetFindFunc;

	protected void Awake()
	{
		_targetArray = new Champion[10];
	}

	private void Start()
	{
		gameObject.tag = "SummonObject";

		if (null != summonObjectManager)
		{
			summonObjectManager.OnForcedRelease -= OnForcedRelease;
			summonObjectManager.OnForcedRelease += OnForcedRelease;

			summonObjectManager.OnReleaseEvent -= ReleaseEvent;
			summonObjectManager.OnReleaseEvent += ReleaseEvent;
        }
	}

	public void ReleaseEvent()
	{
		OnExecuteImpact = null;
		OnRelease = null;
    }

    protected void ReceiveImpactExecuteEvent(int targetCount)
	{
		OnExecuteImpact?.Invoke(this, _targetArray, targetCount);
	}

	protected void ReceiveReleaseEvent()
	{
		OnRelease?.Invoke(this);
	}

	private void OnForcedRelease()
	{
		if (false == gameObject.activeSelf)
			return;

		summonObjectManager.ReleaseSummonObject(this);
	}

	public void Release()
	{
		ReceiveReleaseEvent();
		summonObjectManager.ReleaseSummonObject(this);
	}
}
