using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// è�Ǿ� HP ���¸� ȭ�鿡 �����ִ� UI..
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
