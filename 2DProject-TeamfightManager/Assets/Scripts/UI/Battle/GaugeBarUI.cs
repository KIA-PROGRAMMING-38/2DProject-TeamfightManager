using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챔피언 HP 상태를 화면에 보여주는 UI..
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
