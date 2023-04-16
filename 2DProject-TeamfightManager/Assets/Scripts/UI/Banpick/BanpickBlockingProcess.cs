using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BanpickBlockingProcess : UIBase
{
    public event Action OnEndProcess;

    [SerializeField] private TMP_Text _curStageText;
    [SerializeField] private UIMove _textMoveCopmonent;

    private float _alphaHighlightTime = 1.2f;
    private bool _isTextMove = false;

    private IEnumerator _updateBlockingCoroutine;

    // Start is called before the first frame update
    void Awake()
    {
        _updateBlockingCoroutine = UpdateBlocking();

        _textMoveCopmonent.OnMoveEnd -= OnEndTextMove;
        _textMoveCopmonent.OnMoveEnd += OnEndTextMove;
	}

	private void OnEnable()
	{
        StartCoroutine(_updateBlockingCoroutine);
	}

    public void SetShowBanpickStageKind(BanpickStageKind banpickStageKind)
    {
		switch (banpickStageKind)
		{
			case BanpickStageKind.Ban:
                _curStageText.text = "���� �ܰ�";

				break;
			case BanpickStageKind.Pick:
				_curStageText.text = "���� �ܰ�";

				break;
		}
	}

	private IEnumerator UpdateBlocking()
    {
        while(true)
        {
            // �ʱ�ȭ..
            float curAlpha = 0f;
            float alphaAcc = _alphaHighlightTime / 1f;

            _curStageText.alpha = 0f;
            _curStageText.rectTransform.localPosition = Vector3.zero;

            // �������� ���� �ø���..
            while(true)
            {
                curAlpha += Time.deltaTime * alphaAcc;
                if(curAlpha >= 1f)
                {
					_curStageText.alpha = 1f;

                    break;
				}

                _curStageText.alpha = curAlpha;

                yield return null;
            }

            // �ؽ�Ʈ�� ȭ�� ������ �����δ�..
            _textMoveCopmonent.StartMove(false);
            _isTextMove = true;

            while (true == _isTextMove)
                yield return null;

			// �� �����ߴٸ� Coroutine ����..
			gameObject.SetActive(false);
            OnEndProcess?.Invoke();

			StopCoroutine(_updateBlockingCoroutine);

            yield return null;
		}
    }

    public void OnEndTextMove()
    {
        _isTextMove = false;
	}
}
