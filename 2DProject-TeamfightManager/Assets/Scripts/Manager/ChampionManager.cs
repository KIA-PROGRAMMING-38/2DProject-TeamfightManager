using UnityEngine;
using Util.Pool;

/// <summary>
/// 챔피언을 관리하는 매니저 클래스..
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

	// Champion Pooler에서 챔피언을 생성하기 위해 넘겨줄 메소드..
	private Champion CreateChampion()
	{
		Champion newChampion = Instantiate<Champion>(_championDefaultPrefab);

		DontDestroyOnLoad(newChampion.gameObject);

		newChampion.gameObject.SetActive(false);
		newChampion.championManager = this;
		newChampion.transform.parent = transform;

		return newChampion;
	}

	// 외부에서 챔피언 인스턴스를 받아오려고 할 때 호출될 함수..
    public Champion GetChampionInstance(string championName)
    {
        Champion newChampion = _championPooler.Get();

		ChampionStatus champStatus;
	    ChampionData champData;
	    ChampionAnimData champAnimData;

		_championDataTable.GetChampionAllData(championName, out champData, out champStatus, out champAnimData);

		newChampion.transform.parent = null;
		newChampion.gameObject.SetActive(true);

		newChampion.SetupNecessaryData(champStatus, champData, champAnimData);

		return newChampion;
	}

	public void ReleaseChampion(Champion champion)
	{
		_championPooler.Release(champion);

		champion.gameObject.SetActive(false);
		champion.transform.parent = transform;
	}
}
