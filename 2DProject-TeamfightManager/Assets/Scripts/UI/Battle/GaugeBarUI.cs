using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// è�Ǿ� HP ���¸� ȭ�鿡 �����ִ� UI..
/// </summary>
public class GaugeBarUI : UIBase
{
    private Image _hpRatioGaugeImage;
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
	}

	public void SetFillAmount(float fillAmount)
    {
        _hpRatioGaugeImage.fillAmount = fillAmount;
	}
}
