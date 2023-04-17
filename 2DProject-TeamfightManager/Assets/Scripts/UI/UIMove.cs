using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class UIMove : UIBase
{
    public event Action OnMoveEnd;

    private RectTransform _uiTransform;

    private Vector2 _startPos;
    [SerializeField] private Vector2 _endPos;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField, Range(0.1f, 10f)] private float _duration;

    private float _elaspedTime = 0f;
    private float _multiplyTime = 0f;
    private Func<float, bool> _moveEndCheckFunc;    // 역방향과 정방향 움직임의 종료 조건이 다르기 때문에 움직임 시작 시 가지고 있는다..

	private IEnumerator _updateMoveCoroutine;

    // Start is called before the first frame update
    void Awake()
    {
		_uiTransform = GetComponent<RectTransform>();
        _startPos = _uiTransform.anchoredPosition;

		_updateMoveCoroutine = UpdateMove();
	}

    public void StartMove(bool isReverse)
    {
        if(isReverse)
        {
            _multiplyTime = -1f;
            _elaspedTime = _duration;
            _moveEndCheckFunc = CheckIsEndReverseMove;
		}
        else
        {
			_multiplyTime = 1f;
			_elaspedTime = 0f;
			_moveEndCheckFunc = CheckIsEndMove;
		}

        StartCoroutine(_updateMoveCoroutine);
    }

    // Update is called once per frame
    IEnumerator UpdateMove()
    {
        while(true)
        {
			_elaspedTime += Time.deltaTime * _multiplyTime;
			float runTimeRatio = _elaspedTime / _duration;
			UpdatePosition(runTimeRatio);

            // 종료 조건을 만족한다면 움직임 종료..
            if (true == _moveEndCheckFunc(runTimeRatio))
            {
                OnMoveEnd?.Invoke();
                StopCoroutine(_updateMoveCoroutine);
            }

            yield return null;
		}
    }

    private void UpdatePosition(float runtimeRatio)
    {
        float t = _curve.Evaluate( Mathf.Min(1f, runtimeRatio) );
        _uiTransform.anchoredPosition = Vector2.LerpUnclamped( _startPos, _endPos, t );
    }

    private bool CheckIsEndMove(float ratio)
    {
        return ratio >= 1f;
    }

    private bool CheckIsEndReverseMove(float ratio)
    {
        return ratio <= 0f;
    }
}
