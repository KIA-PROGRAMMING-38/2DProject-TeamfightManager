using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// è�Ǿ� HP ���¸� ȭ�鿡 �����ִ� UI..
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
