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
	}

	private Champion _champion;
	private SpriteRenderer _spriteRenderer;

	private Material _originMaterial;
	[SerializeField] private Material _hitMaterial;

	private Animator _animator;
	private AnimState _state;
	private AnimatorOverrideController _animatorOverrideController;

	// ����, ��ų, �ñر��� �ִϸ��̼� �ð��� ������ ����..
	private float _atkAnimRuntime = 0f;
	private float _skillAnimRuntime = 0f;
	private float _ultAnimRuntime = 0f;

	// �ִϸ����Ϳ��� ���� �Ķ���͵� �̸��� �̸� �ؽð����� ������ �ֱ�(���� �� �Ȱ��� �� ���ϱ� static���� �ؼ� �ϳ��� ����)..
	private static bool s_isHaveKeyHash = false;
	private static int s_isMoveKeyHash = 0;
	private static int s_attackKeyHash = 0;
	private static int s_skillKeyHash = 0;
	private static int s_ultKeyHash = 0;
	private static int s_deathKeyHash = 0;
	private static int s_revivalKeyHash = 0;
	private static int s_onAnimEndKeyHash = 0;

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

		if (false == s_isHaveKeyHash)
		{
			SetupAnimatorKeyToHash();
		}
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
	/// �ִϸ����Ϳ��� ���� �Ķ���͵� �̸��� �ؽð����� ��ȯ�ϴ� �Լ�..
	/// </summary>
	private static void SetupAnimatorKeyToHash()
	{
		s_isMoveKeyHash = Animator.StringToHash(AnimKeyTable.isMove);
		s_attackKeyHash = Animator.StringToHash(AnimKeyTable.onAttack);
		s_skillKeyHash = Animator.StringToHash(AnimKeyTable.onSkill);
		s_ultKeyHash = Animator.StringToHash(AnimKeyTable.onUltimate);
		s_deathKeyHash = Animator.StringToHash(AnimKeyTable.onDeath);
		s_revivalKeyHash = Animator.StringToHash(AnimKeyTable.onRevival);
		s_onAnimEndKeyHash = Animator.StringToHash(AnimKeyTable.onAnimEnd);

		s_isHaveKeyHash = true;
	}

	/// <summary>
	/// �ִϸ��̼� ���� ����..
	/// </summary>
	/// <param name="newState"></param>
	public void ChangeState(AnimState newState)
	{
		if (_state == AnimState.Move && newState != _state)
			_animator.SetBool(s_isMoveKeyHash, false);

		// ���ο� �ִϸ��̼� ���¿� ���� �Ķ���� �� ����..
		switch (newState)
		{
			case AnimState.Idle:
				if (_state == AnimState.Dead)
					_animator.SetTrigger(s_revivalKeyHash);

				break;
			case AnimState.Move:
				_animator.SetBool(s_isMoveKeyHash, true);

				break;
			case AnimState.Attack:
				_animator.SetTrigger(s_attackKeyHash);

				break;
			case AnimState.Skill:
				_animator.SetTrigger(s_skillKeyHash);

				break;
			case AnimState.Ultimate:
				_animator.SetTrigger(s_ultKeyHash);

				break;
			case AnimState.Dead:
				_animator.SetTrigger(s_deathKeyHash);

				break;
		}

		_state = newState;
	}

	/// <summary>
	/// �ִϸ����� �Ķ���� �ʱ�ȭ..
	/// </summary>
	public void ResetAnimation()
	{
		_animator.SetBool(s_isMoveKeyHash, false);
		_animator.ResetTrigger(s_attackKeyHash);
		_animator.ResetTrigger(s_skillKeyHash);
		_animator.ResetTrigger(s_ultKeyHash);
		_animator.ResetTrigger(s_deathKeyHash);
		_animator.ResetTrigger(s_revivalKeyHash);

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
		_animatorOverrideController["Idle"] = animData.idleAnim;
		_animatorOverrideController["Move"] = animData.moveAnim;
		_animatorOverrideController["Attack"] = animData.atkAnim;
		_animatorOverrideController["Skill"] = animData.skillAnim;
		_animatorOverrideController["Ultimate"] = animData.ultAnim;
		_animatorOverrideController["Death"] = animData.deathAnim;
		_animatorOverrideController["DeadLoop"] = animData.deadLoopAnim;

		// �ִϸ��̼� Ŭ���� ���� ����, ��ų, �ñر� �ִϸ��̼� ���� �ð� ���..
		_atkAnimRuntime = animData.atkAnim.length;
		_skillAnimRuntime = animData.skillAnim.length;
		_ultAnimRuntime = animData.ultAnim.length;
	}

	public void OnAnimationEvent(string eventName)
	{
		_champion?.OnAnimEvent(eventName);
	}

	// �ִϸ��̼� �̺�Ʈ : �ִϸ��̼��� ����� �� ȣ��..
	private void OnAnimationEnd()
	{
		_animator.SetTrigger(s_onAnimEndKeyHash);

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
