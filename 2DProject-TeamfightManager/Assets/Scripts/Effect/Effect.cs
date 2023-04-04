using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Effect : MonoBehaviour
{
    public event Action<Effect> OnDisableEvent;

    public static EffectManager s_effectManager { private get; set; }

    private Animator _animator;
    private AnimatorOverrideController _overrideController;
    private SpriteRenderer _spriteRenderer;

    public EffectData info { private get; set; }

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
	}

    private void OnEnable()
    {
        if (null != info)
        {
            Vector3 offsetPosition = info.offsetPos;
            if (_spriteRenderer.flipX)
                offsetPosition.x *= -1f;

			transform.position += offsetPosition;

            _animator.Play("Effect");
        }
    }

	private void OnDisable()
	{
        OnDisableEvent?.Invoke(this);
	}

	public void OnEndAnimation()
    {
        if (true == info.isAutoDestroy && false == info.isUseLifeTime)
        {
            Release();
        }
    }

    public void Release()
    {
		if (false == gameObject.activeSelf)
			return;

		s_effectManager.ReleaseEffect(this);
	}
}
