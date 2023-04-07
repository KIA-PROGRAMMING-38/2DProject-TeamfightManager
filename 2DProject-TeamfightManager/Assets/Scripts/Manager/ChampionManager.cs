using UnityEngine;
using Util.Pool;

/// <summary>
/// è�Ǿ��� �����ϴ� �Ŵ��� Ŭ����..
/// </summary>
public class ChampionManager : MonoBehaviour
{
    private GameManager _gameManager;
    private DataTableManager _dataTableManager;
    private ChampionDataTable _championDataTable;

	public GameManager gameManager
    {
        private get => _gameManager;
        set
        {
            _gameManager = value;

			_dataTableManager = gameManager.dataTableManager;
            _championDataTable = _dataTableManager.championDataTable;
		}
    }

    private ObjectPooler<Champion> _championPooler;

	public Champion _championDefaultPrefab;

	private void Awake()
	{
		_championDefaultPrefab = Resources.Load<GameObject>("Prefabs/Champion").GetComponent<Champion>();
		_championPooler = new ObjectPooler<Champion>(CreateChampion, null, null, null, 8, 8);
	}

	private void Start()
	{

	}

	// Champion Pooler���� è�Ǿ��� �����ϱ� ���� �Ѱ��� �޼ҵ�..
	private Champion CreateChampion()
	{
		Champion newChampion = Instantiate<Champion>(_championDefaultPrefab);

		newChampion.gameObject.SetActive(false);
		newChampion.transform.parent = transform;

		return newChampion;
	}

	// �ܺο��� è�Ǿ� �ν��Ͻ��� �޾ƿ����� �� �� ȣ��� �Լ�..
    public Champion GetChampionInstance(string championName)
    {
        Champion newChampion = _championPooler.Get();

		ChampionStatus champStatus;
	    ChampionData champData;
	    ChampionAnimData champAnimData;

		_championDataTable.GetChampionAllData(championName, out champData, out champStatus, out champAnimData);

        newChampion.SetupNecessaryData(champStatus, champData, champAnimData);

		newChampion.transform.parent = null;
		newChampion.gameObject.SetActive(true);

		return newChampion;
	}
}
