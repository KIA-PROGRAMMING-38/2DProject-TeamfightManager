using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChampInfoButtonUI : UIBase, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public ChampInfoButtonUIManager manager { private get; set; }

	private GameObject _hoverImageObject;

	private Image _champIconImage;
	private TMP_Text _champClassText;
	private TMP_Text _champNameText;
	public int index { get; set; }

	private string _showChampName;
	public string showChampName
	{
		get => _showChampName;
		set
		{
			_showChampName = value;

			ChampionData data = s_dataTableManager.championDataTable.GetChampionData(_showChampName);

			Sprite champIconSprite = s_dataTableManager.championDataTable.GetChampionImage(_showChampName);
			_champIconImage.sprite = champIconSprite;
			UIUtility.CalcSpriteCenterPos(_champIconImage.rectTransform, champIconSprite, _champIconImage.rectTransform.localPosition);

			_champNameText.text = data.nameKR;
			_champClassText.text = ChampionUtility.CalcClassTypeName(data.type);
		}
	}

	private void Awake()
	{
		_hoverImageObject = transform.Find("HoverImage").gameObject;

		_champIconImage = transform.Find("ChampIconImage").GetComponent<Image>();

		_champClassText = transform.Find("ChampClassText").GetComponent<TMP_Text>();
		_champNameText = transform.Find("ChampNameText").GetComponent<TMP_Text>();
	}

	private void OnEnable()
	{
		_hoverImageObject.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_hoverImageObject.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_hoverImageObject.SetActive(false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		manager.OnClickShowChampInfoButton(this);
	}
}
