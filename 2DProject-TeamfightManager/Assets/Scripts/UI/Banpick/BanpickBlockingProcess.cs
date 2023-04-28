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
                _curStageText.text = "금지 단계";

				break;
			case BanpickStageKind.Pick:
				_curStageText.text = "선택 단계";

				break;
		}
	}

	private IEnumerator UpdateBlocking()
    {
        while(true)
        {
			// 초기화..
			s_dataTableManager.battleStageDataTable.isPauseBanpick = true;

			float curAlpha = 0f;
            float alphaAcc = _alphaHighlightTime / 1f;

            _curStageText.alpha = 0f;
            _curStageText.rectTransform.localPosition = Vector3.zero;

            // 불투명도를 점점 올린다..
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

            // 텍스트를 화면 밖으로 움직인다..
            _textMoveCopmonent.StartMove(false);
            _isTextMove = true;

            while (true == _isTextMove)
                yield return null;

			// 다 수행했다면 Coroutine 종료..
			gameObject.SetActive(false);
            OnEndProcess?.Invoke();

			s_dataTableManager.battleStageDataTable.isPauseBanpick = false;

			StopCoroutine(_updateBlockingCoroutine);

            yield return null;
		}
	}

    public void OnEndTextMove()
    {
        _isTextMove = false;
	}
}
