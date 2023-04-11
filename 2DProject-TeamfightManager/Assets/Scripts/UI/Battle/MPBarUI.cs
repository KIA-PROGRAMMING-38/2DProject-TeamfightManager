using UnityEngine;
using UnityEngine.UI;

public class MPBarUI : UIBase
{
    [SerializeField] private Image _gaugeImage;

    public void SetMPRatio(float ratio)
    {
        _gaugeImage.fillAmount = ratio;
    }
}
