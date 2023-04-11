using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챔피언 마나 상태를 화면에 보여주는 UI..
/// </summary>
public class MPBarUI : UIBase
{
    [SerializeField] private Image _gaugeImage;

    public void SetMPRatio(float ratio)
    {
        _gaugeImage.fillAmount = ratio;
    }
}
