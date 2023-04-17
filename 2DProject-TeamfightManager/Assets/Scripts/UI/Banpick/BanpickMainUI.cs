using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BanpickMainUI : UIBase
{
	private UIMove _movementComponent;

    private BanpickChampUIManager _banpickChampUIManager;
	private ShowChampDataUI _showChampDataUI;

	private BattleStageDataTable _battleStageDataTable;
	[SerializeField] private BanpickBlockingProcess _blockingProcess;
	[SerializeField] private BanpickEndUI _banpickEndUI;

	private void Awake()
	{
		// �ʿ��� ������ ���̺� ���� �������� �� �̺�Ʈ ����..
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

		// UI���� �ʿ��� ���� ���� �� �̺�Ʈ ����..
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
		StartCoroutine(WaitBanpickEndDelayTime());
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

	private IEnumerator WaitBanpickEndDelayTime()
	{
		GraphicRaycaster rayCaster = transform.root.GetComponent<GraphicRaycaster>();

		rayCaster.enabled = false;

		yield return YieldInstructionStore.GetWaitForSec(2f);

		rayCaster.enabled = true;
		_movementComponent.StartMove(true);
		_banpickEndUI.OnEndBanpick();
	}
}
