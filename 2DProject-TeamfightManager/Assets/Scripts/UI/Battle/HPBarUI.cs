using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : UIBase
{
    [SerializeField] private Image _hpRatioGaugeImage;
    public Color gaugeBarColor
    {
        set
        {
            _hpRatioGaugeImage.color = value;
        }
    }

    public void SetHPRatio(float hpRatio)
    {
        _hpRatioGaugeImage.fillAmount = hpRatio;
	}
}
