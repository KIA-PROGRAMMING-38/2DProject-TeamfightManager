using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class MenuButtonGroupUI : UIBase, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	private UIMove _subGroupMoveComponent;
	public MenuButtonGroupUIManager manager { private get; set; }

	private bool _isSubgroupActive = false;

	private Image _image;
	private Sprite _defaultStateSprite;
	[SerializeField] private Sprite _selectStateSprite;
	[SerializeField] private Sprite _hoverStateSprite;

	private void Awake()
	{
		_subGroupMoveComponent = GetComponentInChildren<UIMove>();

		_subGroupMoveComponent.OnMoveEnd -= OnSubGroupMoveEnd;
		_subGroupMoveComponent.OnMoveEnd += OnSubGroupMoveEnd;

		_image = GetComponent<Image>();
		_defaultStateSprite = _image.sprite;
	}

	public abstract void OnClickSubGroupButton(int buttonKey);

	public void SetActiveSubGroup(bool isActive)
	{
		_isSubgroupActive = isActive;

		if (true == isActive)
		{
			_subGroupMoveComponent.gameObject.SetActive(true);
			_image.sprite = _selectStateSprite;
		}
		else
		{
			_image.sprite = _defaultStateSprite;
		}

		if (null != _subGroupMoveComponent)
		{
			_subGroupMoveComponent.StartMove(!isActive);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		manager.OnClickButton(this);
	}

	public void OnSubGroupMoveEnd()
	{
		if (false == _isSubgroupActive)
		{
			_subGroupMoveComponent.gameObject.SetActive(false);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (false == _isSubgroupActive)
		{
			_image.sprite = _hoverStateSprite;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (false == _isSubgroupActive)
		{
			_image.sprite = _defaultStateSprite;
		}
	}
}
