using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private GaugeBarUI _hpBarUI;
    [SerializeField] private GaugeBarUI[] _barrierUIBar;

    private float _hpBarLocalStartX;
    private float _hpBarLocalEndX;
    private Vector3 _hpBarStartLocalPosition;

	// Start is called before the first frame update
	void Start()
    {
        RectTransform hpBarTransform = _hpBarUI.rectTransform;

        float hpBarWidth = hpBarTransform.rect.width;
        _hpBarStartLocalPosition = hpBarTransform.localPosition;
		_hpBarLocalStartX = hpBarTransform.localPosition.x;
        _hpBarLocalEndX = _hpBarLocalStartX + hpBarWidth;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHPBarColor(Color color)
    {
		_hpBarUI.gaugeBarColor = color;
	}

    public void SetHPBarGauge(float fillAmount)
    {
        _hpBarUI.fillAmount = fillAmount;
    }

    public void SetBarrierBarGauge(float fillAmount)
    {
        float curHpBarGauge = _hpBarUI.fillAmount;

        float curBarrierStartPosX = Mathf.Lerp(_hpBarLocalStartX, _hpBarLocalEndX, curHpBarGauge);

		Vector3 localPos = _hpBarStartLocalPosition;
		localPos.x = curBarrierStartPosX;
		_barrierUIBar[0].rectTransform.localPosition = localPos;
		_barrierUIBar[0].fillAmount = 1f - curHpBarGauge;

		if (curHpBarGauge + fillAmount > 1f)
        {
            _barrierUIBar[1].gameObject.SetActive(true);
            _barrierUIBar[1].fillAmount = Mathf.Min(1f, fillAmount + curHpBarGauge - 1f);
		}
        else
        {
			_barrierUIBar[1].gameObject.SetActive(false);
		}
    }
}
