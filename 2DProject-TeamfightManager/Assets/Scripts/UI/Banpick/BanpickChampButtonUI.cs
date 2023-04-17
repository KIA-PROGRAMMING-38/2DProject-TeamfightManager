using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BanpickChampButtonUI : UIBase, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public event Action OnButtonClicked;
	public event Action OnStartButtonHover;
	public event Action OnExitButtonHover;

	private Button _buttonComponent;
	private ShowChampAnimUI _champAnimUI;
	[SerializeField] private ShowChampAnimUI _sharedMaterialOwner;

	private void Awake()
	{
        _buttonComponent = GetComponent<Button>();
		_champAnimUI = GetComponentInChildren<ShowChampAnimUI>();
	}

	public void OnButtonSelect()
	{
		GetComponent<Image>().material = _sharedMaterialOwner.material;
	}

    public void OnPointerClick(PointerEventData eventData)
	{
        OnButtonClicked?.Invoke();
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
		//_animator.SetBool(AnimatorHashStore.ON_HOVER_BUTTON, true);
		OnStartButtonHover?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		//_animator.SetBool(AnimatorHashStore.ON_HOVER_BUTTON, false);
		OnExitButtonHover?.Invoke();
	}
}
