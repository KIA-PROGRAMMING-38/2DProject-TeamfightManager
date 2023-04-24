using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챔피언 HP 상태를 화면에 보여주는 UI..
/// </summary>
public class GaugeBarUI : UIBase
{
    private float _gaugeBarFillAmount;
    public float fillAmount
    {
        get => _gaugeBarFillAmount;
        set
        {
            _gaugeBarFillAmount = value;

            _hpRatioGaugeImage.fillAmount = _gaugeBarFillAmount;
		}
	}

    private Image _hpRatioGaugeImage;
    public RectTransform rectTransform { get; private set; }

    public Color gaugeBarColor
    {
        set
        {
            _hpRatioGaugeImage.color = value;
        }
    }

	private void Awake()
	{
		_hpRatioGaugeImage = GetComponent<Image>();
		rectTransform = GetComponent<RectTransform>();
	}
}
