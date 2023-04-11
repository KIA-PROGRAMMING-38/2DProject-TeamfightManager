using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// è�Ǿ� ���� ���¸� ȭ�鿡 �����ִ� UI..
/// </summary>
public class MPBarUI : UIBase
{
    [SerializeField] private Image _gaugeImage;

    public void SetMPRatio(float ratio)
    {
        _gaugeImage.fillAmount = ratio;
    }
}
