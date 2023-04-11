using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챔피언 HP 상태를 화면에 보여주는 UI..
/// </summary>
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
