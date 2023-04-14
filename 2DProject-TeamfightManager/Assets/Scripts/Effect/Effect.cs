using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GridBrushBase;

/// <summary>
/// 이펙트 최상위 클래스(필요하다면 상속받아 재정의 함)..
/// </summary>
public class Effect : MonoBehaviour
{
    public event Action<Effect, string> OnAnimationEvent;
    public event Action<Effect> OnDisableEvent;

    public static EffectManager s_effectManager { private get; set; }

    private Animator _animator;
    private AnimatorOverrideController _overrideController;
    private SpriteRenderer _spriteRenderer;

    public EffectData data { private get; set; }

    private IEnumerator _waitForLifetimeSecondCoroutine;
    private IEnumerator _updateRotateAroundCoroutine;

    private float _rotateSpeed = 0f;
    private float _curAngle = 0f;
	private Transform _targetTransform;

    public AnimationClip clip
    {
        set
        {
            _overrideController["Effect"] = value;
        }
    }

    public bool flipX
    {
        set
        {
            _spriteRenderer.flipX = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
		_animator = GetComponent<Animator>();
        _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _overrideController;

		_spriteRenderer = GetComponent<SpriteRenderer>();

        _waitForLifetimeSecondCoroutine = WaitForLifeTimeSecond();
		_updateRotateAroundCoroutine = UpdateRotateAround();
	}

    private void OnEnable()
    {
        if (null != data)
        {
            Vector3 offsetPosition = data.offsetPos;
            if (_spriteRenderer.flipX)
                offsetPosition.x *= -1f;

            transform.Translate(offsetPosition);

			if (true == data.isUseLifeTime)
			{
                StartCoroutine(_waitForLifetimeSecondCoroutine);
			}

            if (data.rotationType == EffectRotationType.RotateAround)
			{
                _curAngle = 0f;
				_rotateSpeed = data.offsetPos.z * Mathf.Deg2Rad;

				StartCoroutine(_updateRotateAroundCoroutine);
            }
		}

		_animator.Play(AnimatorHashStore.effectKeyHash);
	}

    private void OnDisable()
	{
        StopAllCoroutines();

        OnDisableEvent?.Invoke(this);
		transform.rotation = Quaternion.identity;
	}

    public void ReceiveAnimationEvent(string eventName)
    {
        OnAnimationEvent?.Invoke(this, eventName);
    }

	public void OnEndAnimation()
    {
        if(null != data)
        {
			if (true == data.isAutoDestroy && false == data.isUseLifeTime)
			{
				Release();
			}
		}
    }

    public void Release()
    {
		if (false == gameObject.activeSelf)
			return;

		s_effectManager.ReleaseEffect(this);
	}

    public void SetupAdditionalData(in Vector3 rotationDirection, in Transform targetTransform)
    {
		switch (data.rotationType)
		{
			case EffectRotationType.SettingToOwner:
				transform.right = rotationDirection.normalized;
				_spriteRenderer.flipX = false;
				break;
			case EffectRotationType.RotateAround:
				_targetTransform = targetTransform;
				break;
			case EffectRotationType.Random:
				break;
		}

        if (data.isBecomeTargetChild)
        {
			transform.parent = targetTransform;
            transform.localPosition = Vector3.zero;
		}
	}

    private IEnumerator WaitForLifeTimeSecond()
    {
        while(true)
        {
            yield return YieldInstructionStore.GetWaitForSec(data.lifeTime);

            gameObject.SetActive(false);

            yield return null;
        }
    }

    private IEnumerator UpdateRotateAround()
    {
        Vector3 position = Vector3.zero;

		while (true)
		{
            _curAngle += Time.deltaTime * _rotateSpeed;

            position = _targetTransform.position;
            position.x += Mathf.Cos(_curAngle) * data.offsetPos.x;
            position.y += data.offsetPos.y;
            position.z = MathF.Sin(_curAngle) * data.offsetPos.x;

			transform.position = position;

			yield return null;
		}
	}
}
