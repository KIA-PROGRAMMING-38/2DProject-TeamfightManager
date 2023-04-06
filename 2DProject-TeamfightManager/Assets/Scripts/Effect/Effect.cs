using System;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public event Action<Effect> OnDisableEvent;

    public static EffectManager s_effectManager { private get; set; }

    private Animator _animator;
    private AnimatorOverrideController _overrideController;
    private SpriteRenderer _spriteRenderer;

    public EffectData data { private get; set; }

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
        if (null != data)
        {
            Vector3 offsetPosition = data.offsetPos;
            if (_spriteRenderer.flipX)
                offsetPosition.x *= -1f;

            transform.Translate(offsetPosition);

            _animator.Play("Effect");
        }
    }

	private void OnDisable()
	{
        OnDisableEvent?.Invoke(this);
		transform.rotation = Quaternion.identity;
	}

	public void OnEndAnimation()
    {
        if (true == data.isAutoDestroy && false == data.isUseLifeTime)
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

    public void SetupAdditionalData(in Vector3 rotationDirection)
    {
        if (data.rotationType == EffectRotationType.SettingToOwner)
        {
            transform.right = rotationDirection.normalized;
            _spriteRenderer.flipX = false;
		}
    }
}
