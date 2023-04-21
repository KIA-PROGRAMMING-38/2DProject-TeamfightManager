using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// è�Ǿ��� �ִϸ��̼� �� ��������Ʈ�� ���õ� ��ɵ��� �����ϴ� Ŭ����..
/// </summary>
public class ChampionAnimation : MonoBehaviour
{
	public enum AnimState
	{
		Idle,
		Move,
		Attack,
		Skill,
		Ultimate,
		Dead,
		Revival
	}

	private Champion _champion;
	private SpriteRenderer _spriteRenderer;

	private Material _originMaterial;
	[SerializeField] private Material _hitMaterial;

	private Animator _animator;
	private AnimState _state;
	private AnimatorOverrideController _animatorOverrideController;

	// ������ �ǰ� �� ������� �����Ÿ��� ȿ�� ���� �ʵ�..
	private readonly static float s_hitEffectTime = 0.1f;
	private WaitForSeconds _hitEffectWaitForSecInstance;
	private IEnumerator _showHitMatCoroutine;
	private bool _isRunningHitEvent = false;

	// ���� ���� �ִϸ��̼� ���� �� è�Ǿ𿡰� �� �� �ִٰ� �̺�Ʈ�� �����ϴ� ��� ���� �ʵ�..
	private readonly static float s_delayreceiveAnimEndEventTime = 0.1f;
	private WaitForSeconds _delayReceiveToChampionWaitSecInst;
	private IEnumerator _delayReceiveToChampionCoroutine;

	public bool flipX { get => _spriteRenderer.flipX; set => _spriteRenderer.flipX = value; }

	[SerializeField] private float _animationSpeed;
	public float animationSpeed
	{
		get => _animationSpeed;
		set
		{
			_animationSpeed = Mathf.Min(1.1f, value);

			_animator.SetFloat(AnimatorHashStore.ANIMATION_SPEED, _animationSpeed);
		}
	}

	public ChampionAnimData animData
	{
		set
		{
#if UNITY_EDITOR
			Debug.Assert(null != value);
#endif

			SetupAnimator(value);
		}
	}

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_champion = GetComponentInParent<Champion>();

		_originMaterial = _spriteRenderer.material;
	}

	void Start()
    {
		_hitEffectWaitForSecInstance = YieldInstructionStore.GetWaitForSec(s_hitEffectTime);
		_delayReceiveToChampionWaitSecInst = YieldInstructionStore.GetWaitForSec(s_delayreceiveAnimEndEventTime);

		_showHitMatCoroutine = ShowHitMaterial();
		_delayReceiveToChampionCoroutine = StartReceiveEventDelay();
	}

	private void OnDisable()
	{
		_spriteRenderer.material = _originMaterial;
		animationSpeed = 1f;
	}

	public void OnHit()
	{
		if (false == _isRunningHitEvent)
			StartCoroutine(_showHitMatCoroutine);
	}

	IEnumerator ShowHitMaterial()
	{
		while(true)
		{
			_isRunningHitEvent = true;

			_spriteRenderer.material = _hitMaterial;
			yield return _hitEffectWaitForSecInstance;
			_spriteRenderer.material = _originMaterial;

			StopCoroutine(_showHitMatCoroutine);

			_isRunningHitEvent = false;
			yield return null;
		}
	}

	/// <summary>
	/// �ִϸ��̼� ���� ����..
	/// </summary>
	/// <param name="newState"></param>
	public void ChangeState(AnimState newState, bool isForceChange = false)
	{
		if (_state == AnimState.Move && newState != _state)
			_animator.SetBool(AnimatorHashStore.IS_MOVE_KEY_HASH, false);

		// ���ο� �ִϸ��̼� ���¿� ���� �Ķ���� �� ����..
		switch (newState)
		{
			case AnimState.Idle:
				_animator.SetBool(AnimatorHashStore.IS_MOVE_KEY_HASH, false);

				break;
			case AnimState.Move:
				_animator.SetBool(AnimatorHashStore.IS_MOVE_KEY_HASH, true);

				break;
			case AnimState.Attack:
				if (true == isForceChange)
					_animator.SetTrigger(AnimatorHashStore.ON_ANIMEND_KEY_HASH);
				_animator.SetTrigger(AnimatorHashStore.ATTACK_KEY_HASH);

				break;
			case AnimState.Skill:
				if (true == isForceChange)
					_animator.SetTrigger(AnimatorHashStore.ON_ANIMEND_KEY_HASH);
				_animator.SetTrigger(AnimatorHashStore.SKILL_KEY_HASH);

				break;
			case AnimState.Ultimate:
				if (true == isForceChange)
					_animator.SetTrigger(AnimatorHashStore.ON_ANIMEND_KEY_HASH);
				_animator.SetTrigger(AnimatorHashStore.ULT_KEY_HASH);

				break;
			case AnimState.Dead:
				_animator.SetTrigger(AnimatorHashStore.DEATH_KEY_HASH);

				break;

			case AnimState.Revival:
				_animator.SetTrigger(AnimatorHashStore.REVIVAL_KEY_HASH);
				ChangeState(AnimState.Idle);

				return;
		}

		_state = newState;
	}

	/// <summary>
	/// �ִϸ����� �Ķ���� �ʱ�ȭ..
	/// </summary>
	public void ResetAnimation()
	{
		_animator.SetBool(AnimatorHashStore.IS_MOVE_KEY_HASH, false);
		_animator.ResetTrigger(AnimatorHashStore.ATTACK_KEY_HASH);
		_animator.ResetTrigger(AnimatorHashStore.SKILL_KEY_HASH);
		_animator.ResetTrigger(AnimatorHashStore.ULT_KEY_HASH);
		_animator.ResetTrigger(AnimatorHashStore.DEATH_KEY_HASH);
		_animator.ResetTrigger(AnimatorHashStore.REVIVAL_KEY_HASH);

		ChangeState(AnimState.Idle);
	}

	/// <summary>
	/// �ִϸ������� �ִϸ��̼� Ŭ������ ���� è�Ǿ� �´� �ִϸ��̼����� ��ȯ�ϴ� ����..
	/// </summary>
	/// <param name="animData"></param>
	private void SetupAnimator( ChampionAnimData animData )
    {
#if UNITY_EDITOR
		Debug.Assert(null != _animator);
#endif

		// ���� �ִϸ����� ��Ʈ�ѷ��� �������̵��� ĳ���Ϳ� �´� �ִϸ��̼��� �������ֱ� ���� ������Ʈ ����..
		if (null == _animatorOverrideController)
		{
			_animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

			// ���� ������ ������Ʈ�� �۵���Ű���� �Ѵ�..
			_animator.runtimeAnimatorController = _animatorOverrideController;
		}

		// �ִϸ������� Ŭ������ ���� è�Ǿ� �´� �ִϸ��̼� Ŭ����� ����..
		_animatorOverrideController["Champion_Idle"] = animData.idleAnim;
		_animatorOverrideController["Champion_Move"] = animData.moveAnim;
		_animatorOverrideController["Champion_Attack"] = animData.atkAnim;
		_animatorOverrideController["Champion_Skill"] = animData.skillAnim;
		_animatorOverrideController["Champion_Ultimate"] = animData.ultAnim;
		_animatorOverrideController["Champion_Death"] = animData.deathAnim;
		_animatorOverrideController["Champion_DeadLoop"] = animData.deadLoopAnim;
	}

	public void OnAnimationEvent(string eventName)
	{
		_champion?.OnAnimEvent(eventName);
	}

	public void OnShowEffectAnimEvent(string effectName)
	{
		_champion?.OnShowEffectAnimEvent(effectName);
	}

	// �ִϸ��̼� �̺�Ʈ : �ִϸ��̼��� ����� �� ȣ��..
	public void OnAnimationEnd()
	{
		_animator.SetTrigger(AnimatorHashStore.ON_ANIMEND_KEY_HASH);

		StartCoroutine(_delayReceiveToChampionCoroutine);
	}

	// �ִϸ��̼��� �����ڸ��� è�Ǿ𿡰� ���� ����� �������� �ʰ� �����̸� �ֱ� ���� �ڷ�ƾ..
	IEnumerator StartReceiveEventDelay()
	{
		while(true)
		{
			yield return _delayReceiveToChampionWaitSecInst;
			OnAnimationEvent("OnAnimEnd");

			StopCoroutine(_delayReceiveToChampionCoroutine);

			yield return null;
		}
	}
}
