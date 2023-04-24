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

    private float _totalBarrierGauge = 0f;

	void Start()
    {
        RectTransform hpBarTransform = _hpBarUI.rectTransform;

        float hpBarWidth = hpBarTransform.rect.width;
        _hpBarStartLocalPosition = hpBarTransform.localPosition;
		_hpBarLocalStartX = hpBarTransform.localPosition.x;
        _hpBarLocalEndX = _hpBarLocalStartX + hpBarWidth;
	}

    public void SetHPBarColor(Color color)
    {
		_hpBarUI.gaugeBarColor = color;
	}

    public void SetHPBarGauge(float fillAmount)
    {
        _hpBarUI.fillAmount = fillAmount;

		UpdateBarrierGague();
	}

    public void SetBarrierBarGauge(float fillAmount)
    {
        _totalBarrierGauge = fillAmount;

        UpdateBarrierGague();
	}

	// Barrier Gauge ��ġ �������ϴ� �Լ�..
    private void UpdateBarrierGague()
    {
		float curHpBarGauge = _hpBarUI.fillAmount;

		// HP Bar ������ ������ ��ġ ���..
		float curBarrierStartPosX = Mathf.Lerp(_hpBarLocalStartX, _hpBarLocalEndX, curHpBarGauge);

		// Barrier ��ġ ���� �� FillAmount ä���..
		Vector3 localPos = _hpBarStartLocalPosition;
		localPos.x = curBarrierStartPosX;
		_barrierUIBar[0].rectTransform.localPosition = localPos;
		_barrierUIBar[0].fillAmount = Mathf.Min(1f - curHpBarGauge, _totalBarrierGauge);

		// Barrier �������� ü�¹� ���� �� ������ġ�� �ϳ��� Barrier �������� �� �����ش�..
		if (curHpBarGauge + _totalBarrierGauge > 1f)
		{
			_barrierUIBar[1].gameObject.SetActive(true);
			_barrierUIBar[1].fillAmount = Mathf.Min(1f, _totalBarrierGauge + curHpBarGauge - 1f);
		}
		else
		{
			_barrierUIBar[1].gameObject.SetActive(false);
		}
	}
}
