using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 궁극기 아이콘 및 사용 여부를 화면에 보여주는 UI..
/// </summary>
public class UltimateIconUI : UIBase
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _backgroundImage;
	[SerializeField] private Color _activeOffColor;
    [SerializeField] private Sprite _deactiveBackgroundSprite;

    public void SetUltimateIconSprite(Sprite sprite)
    {
        _iconImage.sprite = sprite;
	}

    public void SetIconActive(bool active)
    {
        if(!active)
        {
			_backgroundImage.sprite = _deactiveBackgroundSprite;
			_iconImage.color = _activeOffColor;
		}
	}
}
