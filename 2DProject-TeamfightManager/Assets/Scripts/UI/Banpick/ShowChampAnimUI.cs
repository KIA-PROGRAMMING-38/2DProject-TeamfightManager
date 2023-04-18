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

	[System.Serializable]
	public class AdditiveColorHighlightData
	{
		public Color color;
		public float startIntensity;
		public float endIntensity;
		public float intensityHighlightSpeed;
		public bool isHighlightIntensity;

		public Color emissiveColor;
		public float startEmissiveIntensity;
		public float endEmissiveIntensity;
		public float emissiveHighlightSpeed;
		public bool isHighlightEmissive;

		public bool isPingPong;
	}

	public class ColorHighlightCurData
	{
		public float intensityAcc;
		public float emissiveAcc;

		public float curIntensity;
		public float curEmissive;
	}

	private Image _iconImage;
	public Material material { get; private set; }

	[SerializeField] private AdditiveColorHighlightData _banColorHighlight;
	[SerializeField] private AdditiveColorHighlightData _redTeamPickColorHighlight;
	[SerializeField] private AdditiveColorHighlightData _blueTeamPickColorHighlight;
	[SerializeField] private AdditiveColorHighlightData _hoverColorHighlight;

	private AdditiveColorHighlightData _curColorHighlight;
	private ColorHighlightCurData _curHighlightInfo;

	private void Awake()
	{
		_iconImage = GetComponent<Image>();
		material = Instantiate(_iconImage.material);
		_iconImage.material = material;

		_curHighlightInfo = new ColorHighlightCurData();
	}

	private void Update()
	{
		if (null != _curColorHighlight)
		{
			if(_curColorHighlight.isHighlightIntensity)
			{
				_curHighlightInfo.curIntensity += _curHighlightInfo.intensityAcc * Time.deltaTime;

				if (_curColorHighlight.startIntensity >= _curHighlightInfo.curIntensity || _curColorHighlight.endIntensity <= _curHighlightInfo.curIntensity)
				{
					_curHighlightInfo.curIntensity =
						Mathf.Clamp(_curHighlightInfo.curIntensity, _curColorHighlight.startIntensity, _curColorHighlight.endIntensity);

					if (_curColorHighlight.isPingPong)
						_curHighlightInfo.intensityAcc *= -1f;
				}

				material.SetFloat(ShaderParamtable.UIAdditive.INTENSITY, _curHighlightInfo.curIntensity);
			}

			if (_curColorHighlight.isHighlightEmissive)
			{
				_curHighlightInfo.curEmissive += _curHighlightInfo.emissiveAcc * Time.deltaTime;

				if (_curColorHighlight.startEmissiveIntensity >= _curHighlightInfo.curEmissive || _curColorHighlight.endEmissiveIntensity <= _curHighlightInfo.curEmissive)
				{
					_curHighlightInfo.curEmissive =
						Mathf.Clamp(_curHighlightInfo.curEmissive, _curColorHighlight.startEmissiveIntensity, _curColorHighlight.endEmissiveIntensity);

					if (_curColorHighlight.isPingPong)
						_curHighlightInfo.emissiveAcc *= -1f;
				}

				material.SetFloat(ShaderParamtable.UIAdditive.EMISSIVEINTENSITY, _curHighlightInfo.curEmissive);
			}
		}
	}

	public void SetChampionName(string championName)
	{
		ChampionDataTable dataTable = s_dataTableManager.championDataTable;

		Sprite sprite = dataTable.GetChampionImage(championName);
		_iconImage.sprite = sprite;

		UIUtility.CalcSpriteCenterPos(_iconImage.rectTransform, sprite, _iconImage.rectTransform.localPosition);
	}

	public void SetHoverState(bool isHover)
	{
		if(isHover)
		{
			ChangeColorHighlight(_hoverColorHighlight);
		}
		else
		{
			ChangeColorHighlight(null);
		}
	}

	public void ChangeBanPickData(BanpickStageKind stageKind, BattleTeamKind teamKind)
	{
		switch (stageKind)
		{
			case BanpickStageKind.Ban:
				ChangeColorHighlight(_banColorHighlight);

				break;
			case BanpickStageKind.Pick:
				if (teamKind == BattleTeamKind.BlueTeam)
				{
					ChangeColorHighlight(_blueTeamPickColorHighlight);
				}
				else if (teamKind == BattleTeamKind.RedTeam)
				{
					ChangeColorHighlight(_redTeamPickColorHighlight);
				}

				break;
		}
	}

	public void ChangeColorHighlight(AdditiveColorHighlightData data)
	{
		_curColorHighlight = data;

		if (null == _curColorHighlight)
		{
			material.SetFloat(ShaderParamtable.UIAdditive.INTENSITY, 1f);
			material.SetFloat(ShaderParamtable.UIAdditive.EMISSIVEINTENSITY, 0f);
			material.SetColor(ShaderParamtable.UIAdditive.COLOR, Color.white);
			material.SetColor(ShaderParamtable.UIAdditive.EMISSIVECOLOR, Color.white);

			return;
		}

		material.SetColor(ShaderParamtable.UIAdditive.COLOR, _curColorHighlight.color);
		material.SetColor(ShaderParamtable.UIAdditive.EMISSIVECOLOR, _curColorHighlight.emissiveColor);

		_curHighlightInfo.intensityAcc = _curColorHighlight.intensityHighlightSpeed * Mathf.Abs(_curColorHighlight.endIntensity - _curColorHighlight.startIntensity);
		_curHighlightInfo.emissiveAcc = _curColorHighlight.emissiveHighlightSpeed * Mathf.Abs(_curColorHighlight.endEmissiveIntensity - _curColorHighlight.startEmissiveIntensity);

		_curHighlightInfo.curIntensity = _curColorHighlight.startIntensity;
		_curHighlightInfo.curEmissive = _curColorHighlight.startEmissiveIntensity;
	}
}
