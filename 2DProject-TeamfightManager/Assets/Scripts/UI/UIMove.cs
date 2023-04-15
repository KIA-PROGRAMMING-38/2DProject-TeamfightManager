using System;
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

    // Start is called before the first frame update
    void Awake()
    {
        _uiTransform = GetComponent<RectTransform>();
        _startPos = _uiTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        _elaspedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        _elaspedTime += Time.deltaTime;
        float runTimeRatio = _elaspedTime / _duration;
        UpdatePosition( runTimeRatio );

        if(runTimeRatio >= 1f)
        {
            OnMoveEnd?.Invoke();
            enabled = false;
        }
    }

    private void UpdatePosition(float runtimeRatio)
    {
        float t = _curve.Evaluate( Mathf.Min(1f, runtimeRatio) );
        _uiTransform.anchoredPosition = Vector2.LerpUnclamped( _startPos, _endPos, t );
    }
}
