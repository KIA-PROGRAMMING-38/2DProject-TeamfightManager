using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BanpickChampButtonUI : UIBase, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public event Action OnButtonClicked;

	private Button _buttonComponent;

	private Animator _animator;
	private AnimatorOverrideController _overrideController;
	private float _clickAnimPlayTime = 0f;

	private void Awake()
	{
        // Animator override Controller 생성 후 애니메이터에 재생중인 컨트롤러 교체..
        //_animator = GetComponent<Animator>();
        //_overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        //_animator.runtimeAnimatorController = _overrideController;

        _buttonComponent = GetComponent<Button>();
    }

	public void SetChampionName(string championName)
	{
		ChampionDataTable dataTable = s_dataTableManager.championDataTable;

		ChampionAnimData animData = dataTable.GetChampionAnimData(championName);

		_overrideController["Idle"] = animData.idleAnim;
		_overrideController["Hover"] = animData.moveAnim;
		_overrideController["Click"] = animData.atkAnim;

		_clickAnimPlayTime = animData.atkAnim.length;
	}

	public void SetButtonActive(bool isActive)
	{
		_buttonComponent.enabled = isActive;
	}

	public void ChangeBanpickState(BanpickStageKind state)
	{
        
    }


    public void OnPointerClick(PointerEventData eventData)
	{
        //OnButtonClicked?.Invoke();
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
		//_animator.SetBool(AnimatorHashStore.ON_HOVER_BUTTON, true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
        //_animator.SetBool(AnimatorHashStore.ON_HOVER_BUTTON, false);
    }

	public void SetState(BanpickStageKind kind)
	{
		switch (kind)
		{
			case BanpickStageKind.Ban:

				break;
			case BanpickStageKind.Pick:
				_animator.SetBool(AnimatorHashStore.ON_SELECT_BUTTON, true);
				StartCoroutine(WaitForClickAnimationEnd());

				break;
		}
	}

	IEnumerator WaitForClickAnimationEnd()
	{
		yield return YieldInstructionStore.GetWaitForSec(_clickAnimPlayTime);

		_animator.SetBool(AnimatorHashStore.ON_SELECT_BUTTON, false);
	}
}
