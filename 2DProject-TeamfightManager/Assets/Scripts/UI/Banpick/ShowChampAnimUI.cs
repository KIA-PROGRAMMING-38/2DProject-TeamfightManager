using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowChampAnimUI : UIBase
{
	public enum AnimState
	{
		Idle,
		Hover,
		Click
	}

	private Image _iconImage;

	private bool _isRotate;

	[SerializeField] private RectTransform _rotateTransform;
	[SerializeField, Range(150f, 300f)] private float _rotateSpeed = 197f;

	private void Awake()
	{
		_iconImage = GetComponent<Image>();
	}

	private void Update()
	{
		if (true == _isRotate)
		{
			_rotateTransform.Rotate(new Vector3(0f, Time.deltaTime * 197f, 0f));
			Debug.Log("Rotate Сп");
		}
	}

	public void SetChampionName(string championName)
	{
		ChampionDataTable dataTable = s_dataTableManager.championDataTable;

		Sprite sprite = dataTable.GetChampionImage(championName);
		_iconImage.sprite = sprite;

		UIUtility.CalcSpriteCenterPos(_iconImage.rectTransform, sprite, _iconImage.rectTransform.localPosition);
	}

	public void ChangeRotateState(bool isRotate)
	{
		_isRotate = isRotate;
		if(false == _isRotate)
		{
			_rotateTransform.rotation = Quaternion.identity;
		}
	}
}
