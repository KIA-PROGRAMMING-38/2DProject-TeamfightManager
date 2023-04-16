using UnityEngine;

public class BanpickMainUI : UIBase
{
	private UIMove _movementComponent;

    private BanpickChampUIManager _banpickChampUIManager;
	private ShowChampDataUI _showChampDataUI;

	private BattleStageDataTable _battleStageDataTable;
	[SerializeField] private BanpickBlockingProcess _blockingProcess;

	private void Awake()
	{
		// 필요한 데이터 테이블 참조 가져오기 및 이벤트 구독..
		_battleStageDataTable = s_dataTableManager.battleStageDataTable;
		_battleStageDataTable.OnBanpickEnd -= OnBanpickEnd;
		_battleStageDataTable.OnBanpickEnd += OnBanpickEnd;

		_battleStageDataTable.OnBanpickOneStageStart -= OnBanpickOneStageStart;
		_battleStageDataTable.OnBanpickOneStageStart += OnBanpickOneStageStart;

		_banpickChampUIManager = GetComponentInChildren<BanpickChampUIManager>();
		_showChampDataUI = GetComponentInChildren<ShowChampDataUI>();
		_movementComponent = GetComponent<UIMove>();

		_movementComponent.OnMoveEnd -= OnMoveEnd;
		_movementComponent.OnMoveEnd += OnMoveEnd;

		// UI에게 필요한 참조 전달 및 이벤트 구독..
		_banpickChampUIManager.banpickMainUI = this;
		_banpickChampUIManager.OnSelectChampionButton -= OnClickSelectChampionButton;
		_banpickChampUIManager.OnSelectChampionButton += OnClickSelectChampionButton;

		_blockingProcess.OnEndProcess -= OnEndBlockingProcess;
		_blockingProcess.OnEndProcess += OnEndBlockingProcess;

		_blockingProcess.gameObject.SetActive(false);
	}

	private void Start()
	{
		ChangeShowChampionData(s_dataTableManager.championDataTable.GetChampionName(0));
		_movementComponent.StartMove(false);
	}

	public void ChangeShowChampionData(string championName)
    {
		_showChampDataUI.ChangeChampionData(championName);
	}

	private void OnClickSelectChampionButton(string championName)
	{
		_battleStageDataTable.OnClickedSelectChampButton(championName);
	}

	private void OnBanpickEnd()
	{
		//transform.root.GetComponent<GraphicRaycaster>().enabled = false;
		_movementComponent.StartMove(true);
	}

	private void OnMoveEnd()
	{
		OnBanpickOneStageStart(_battleStageDataTable.curBanpickStageInfo.stageKind);
	}

	private void OnEndBlockingProcess()
	{

	}

	private void OnBanpickOneStageStart(BanpickStageKind curStageKind)
	{
		_blockingProcess.SetShowBanpickStageKind(curStageKind);
		_blockingProcess.gameObject.SetActive(true);
	}
}
