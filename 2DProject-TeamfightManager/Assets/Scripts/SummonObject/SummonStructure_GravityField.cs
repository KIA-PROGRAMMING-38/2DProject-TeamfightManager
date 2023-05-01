using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonStructure_GravityField : SummonStructure
{
	private const int ACTION_COUNT = 4;
	private const float ACTION_TIME = 0.6f;

	private static readonly int s_OnBreakAnimHash = Animator.StringToHash("OnBreak");

	private Collider2D _collider;
	private Animator _circleAnimator;
	private Animator _swordAnimator;

	private WaitForSeconds _waitTickSecInst;
	private WaitForSeconds _waitActionEndSecInst;

	private int _findTargetCount = 0;

	private AudioClip _startClip;
	private AudioClip _impactClip;

	private Vector3[] _actionEndPositionArray;

	new private void Awake()
	{
		base.Awake();

        _collider = GetComponentInChildren<Collider2D>();
		_circleAnimator = transform.Find("Circle").GetComponent<Animator>();
		_swordAnimator = transform.Find("Sword").GetComponent<Animator>();

		_waitTickSecInst = YieldInstructionStore.GetWaitForSec(_actionTickTime);
		_waitActionEndSecInst = YieldInstructionStore.GetWaitForSec(0.5f);

		_actionEndPositionArray = new Vector3[8];

		_startClip = SoundStore.GetAudioClip("Magicknight_UltimateStart");
        _impactClip = SoundStore.GetAudioClip("Magicknight_UltimateImpact");
    }

    private void OnEnable()
    {
		_audioSource.PlayOneShot(_startClip);
    }

    private void OnDisable()
    {
		_audioSource.Stop();
    }

    public override void SetAdditionalData(int layerMask, Champion target, Champion owner, Func<Vector3, Champion[], int> targetFindFunc)
	{
		base.SetAdditionalData(layerMask, target, owner, targetFindFunc);

		gameObject.layer = LayerTable.Number.CalcOtherTeamLayer(layerMask);
		StartCoroutine(UpdateLogic());
	}

	// 적을 끌어당기고 데미지를 주는 로직..
	protected override void Action()
	{
		_findTargetCount = _targetFindFunc.Invoke(transform.position, _targetArray);
		SetupActionEndPosition();

		ReceiveImpactExecuteEvent(_findTargetCount);

		StartCoroutine(UpdateAction());
	}

	// 중력장에서 해야할 행동들 실행하는 코루틴..
	private IEnumerator UpdateLogic()
	{
		for( int i = 1; i < ACTION_COUNT; ++i)
		{
            _audioSource.PlayOneShot(_impactClip);

            this.Action();

			yield return _waitTickSecInst;
		}

		this.Action();

		yield return YieldInstructionStore.GetWaitForSec(ACTION_TIME);

		_swordAnimator.SetTrigger(s_OnBreakAnimHash);
	}

	// 적을 끌어당기는 로직 코루틴..
	private IEnumerator UpdateAction()
	{
		_collider.enabled = true;
		_circleAnimator.gameObject.SetActive(true);

		float lerpT = 0f;
		while (true)
		{
			float curT = Time.deltaTime / ACTION_TIME;
			lerpT += curT;

			for (int i = 0; i < _findTargetCount; ++i)
			{
				if (null == _targetArray[i] || _targetArray[i].isDead)
				{
					continue;
				}

				_targetArray[i].transform.position = Vector3.Lerp(_targetArray[i].transform.position, _actionEndPositionArray[i], curT * 1.2f);
			}

			if (lerpT >= 1f)
			{
				break;
			}

			yield return null;
		}

		_circleAnimator.gameObject.SetActive(false);
		_collider.enabled = false;
	}

	public void OnEndSwordBreakAnimation()
	{
		ReceiveReleaseEvent();
		summonObjectManager.ReleaseSummonObject(this);
	}

	private void SetupActionEndPosition()
	{
		for( int i = 0; i < _findTargetCount; ++i)
		{
			Vector3 direction = transform.position - _targetArray[i].transform.position;
			float distance = direction.magnitude;
			float moveDistance = Mathf.Min(distance, Mathf.Lerp(1f, 1.5f, 2.5f - distance));

			_actionEndPositionArray[i] = _targetArray[i].transform.position + direction.normalized * moveDistance;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Champion champion = collision.GetComponent<Champion>();
		if (null == champion)
			return;

		for ( int i = 0; i < _findTargetCount; ++i)
		{
			if (_targetArray[i] == champion)
			{
				_targetArray[i] = null;
				break;
			}
		}
	}
}
